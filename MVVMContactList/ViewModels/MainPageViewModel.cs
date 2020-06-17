using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using MVVMContactList.Annotations;
using MVVMContactList.Converters;
using MVVMContactList.Utils;
using VkNetModel = VkNet.Model;

namespace MVVMContactList.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public class ContactsSource : IIncrementalSource<Contact>
        {
            private readonly List<VkNetModel.User> _usersList;

            private static readonly VkUserToContactConverter _converter = new VkUserToContactConverter();

            public ContactsSource(IEnumerable<VkNetModel.User> usersList)
            {
                _usersList = usersList.ToList();
            }

            public async Task<IEnumerable<Contact>> GetPagedItemsAsync(int pageIndex, int pageSize,
                CancellationToken cancellationToken = new CancellationToken())
            {
                // simulate long work
                await Task.Delay(500, cancellationToken);
                return await Task.Factory.StartNew(() => _usersList
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .Select(user => (Contact) _converter.Convert(user, typeof(Contact), null, string.Empty)),
                    cancellationToken);
            }
        }

        private ContactsSource _contactsSource;
        private Contact _selectedContact;
        public IncrementalLoadingCollection<ContactsSource, Contact> ContactsCollection { get; private set; }

        public Contact SelectedContact
        {
            get => _selectedContact;
            // bound in xaml, don't make private
            set
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
            }
        }

        private async void FillContactsList()
        {
            // method is async void so it can be run in constructor
            _contactsSource = new ContactsSource(
                new List<VkNetModel.User>(
                    await VkUtils.GetFriendsWithDefaultFields()));
            ContactsCollection = new IncrementalLoadingCollection<ContactsSource, Contact>(_contactsSource, 5);
            OnPropertyChanged(nameof(ContactsCollection));
        }

        public MainPageViewModel()
        {
            FillContactsList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
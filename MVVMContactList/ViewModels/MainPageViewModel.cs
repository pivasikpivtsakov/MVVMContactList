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

namespace MVVMContactList.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public class ContactsSource : IIncrementalSource<Contact>
        {
            private readonly List<Contact> _contactsList;

            public ContactsSource(IEnumerable<Contact> contactsList)
            {
                _contactsList = contactsList.ToList();
            }

            public async Task<IEnumerable<Contact>> GetPagedItemsAsync(int pageIndex, int pageSize,
                CancellationToken cancellationToken = new CancellationToken())
            {
                // simulate long work
                await Task.Delay(1000, cancellationToken);
                return await Task.Factory.StartNew(() => _contactsList.Skip(pageIndex * pageSize).Take(pageSize),
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
            var converter = new VkUserToContactConverter();
            _contactsSource = new ContactsSource(
                new List<Contact>(
                    (await VkUtils.GetAllFriendsOr5000WithDefaultFields())
                    .Select(x =>
                        (Contact) converter.Convert(x, typeof(Contact), null, string.Empty))));
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
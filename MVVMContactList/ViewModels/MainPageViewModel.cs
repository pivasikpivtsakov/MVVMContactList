using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using MVVMContactList.Annotations;
using MVVMContactList.Common;
using MVVMContactList.Utils;
using VkNetModel = VkNet.Model;

namespace MVVMContactList.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public class ShowableUsersSource : IIncrementalSource<ShowableUser>
        {
            private readonly List<VkNetModel.User> _usersList;

            public ShowableUsersSource(IEnumerable<VkNetModel.User> usersList)
            {
                _usersList = usersList.ToList();
            }

            public async Task<IEnumerable<ShowableUser>> GetPagedItemsAsync(int pageIndex, int pageSize,
                CancellationToken cancellationToken = new CancellationToken())
            {
                var pagedUsers = _usersList
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
                var pagedShowableUsers = new List<ShowableUser>();
                foreach (var user in pagedUsers)
                {
                    var showableUser = new ShowableUser(user);
                    await showableUser.LoadImageToSource();
                    pagedShowableUsers.Add(showableUser);
                }

                return pagedShowableUsers;
            }
        }

        private ShowableUsersSource _showableUsersSource;
        private ShowableUser _selectedShowableUser;
        public IncrementalLoadingCollection<ShowableUsersSource, ShowableUser> ShowableUsersCollection { get; private set; }

        public ShowableUser SelectedShowableUser
        {
            get => _selectedShowableUser;
            // bound in xaml, don't make private
            set
            {
                _selectedShowableUser = value;
                OnPropertyChanged(nameof(SelectedShowableUser));
            }
        }

        private async void FillContactsList()
        {
            // method is async void so it can be run in constructor
            _showableUsersSource = new ShowableUsersSource(
                new List<VkNetModel.User>(
                    await VkUtils.GetFriendsWithDefaultFields()));
            ShowableUsersCollection =
                new IncrementalLoadingCollection<ShowableUsersSource, ShowableUser>(_showableUsersSource, 5);
            OnPropertyChanged(nameof(ShowableUsersCollection));
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
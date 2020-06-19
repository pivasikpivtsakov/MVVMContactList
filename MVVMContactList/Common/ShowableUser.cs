using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using MVVMContactList.Annotations;
using VkNetModel = VkNet.Model;

namespace MVVMContactList.Common
{
    public class ShowableUser
    {
        public VkNetModel.User User { get; set; }

        public string UserReadableName => User.FirstName + " " + User.LastName;
        
        public ImageSource ImageSource { get; set; }

        public async Task LoadImageToSource()
        {
            ImageSource = await ImageUtils.ConvertToImageSource(await ImageUtils.UriToByte(User.Photo50));
        }
        
        public ShowableUser([NotNull] VkNetModel.User user)
        {
            User = user;
        }
    }
}
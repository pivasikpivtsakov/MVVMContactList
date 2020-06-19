using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using VkNetModel = VkNet.Model;
using Contact = Windows.ApplicationModel.Contacts.Contact;

namespace MVVMContactList.Converters
{
    public class VkUserToContactWithCachedThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var user = (VkNetModel.User) value;
            var contact = new Contact
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Thumbnail = RandomAccessStreamReference.CreateFromUri(user.Photo50)
            };
            return contact;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
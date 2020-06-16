using VkNet;
using VkNet.Model;

namespace MVVMContactList.Common
{
    public static class VkObjects
    {
        public static VkApi Api { get; private set; }

        public static void InitializeApi(string token)
        {
            Api = new VkApi();
            Api.Authorize(new ApiAuthParams
                {
                    AccessToken = token
                });
        }
    }
}
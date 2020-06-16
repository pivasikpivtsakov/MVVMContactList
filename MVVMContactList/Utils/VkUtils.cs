﻿using System.Threading.Tasks;
using MVVMContactList.Common;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace MVVMContactList.Utils
{
    public static class VkUtils
    {
        public static async Task<VkCollection<User>> GetAllFriendsOr5000WithDefaultFields(int offset = 0)
        {
            return await VkObjects.Api.Friends.GetAsync(new FriendsGetParams
            {
                Fields = ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.ScreenName |
                         ProfileFields.Photo50
            });
        }
    }
}
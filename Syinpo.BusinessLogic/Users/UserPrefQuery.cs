using Syinpo.BusinessLogic.Caches;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Mapper;
using Syinpo.Model.Dto.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Syinpo.BusinessLogic.Users
{
    public partial class UserService
    {
        public async Task<UserDto> GetUserById_Pref(int id)
        {
            if (id == 0)
                return null;

            return await _cache.HGetSet(
                CacheKeys.User_Hash, string.Format(CacheKeys.User_Hash_Id, id),
                async () =>
                {
                    var user = await GetUserById(id);
                    return user?.MapTo<UserDto>();
                },
                60 * 24);
        }

        public async Task<UserDto> GetUserByUsername_Pref(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return await _cache.HGetSet(
                CacheKeys.User_Hash, string.Format(CacheKeys.User_Hash_Name, userName),
                async () =>
                {
                    var user = await GetUserByUsername(userName);
                    return user?.MapTo<UserDto>();
                },
                60 * 24);
        }
    }
}

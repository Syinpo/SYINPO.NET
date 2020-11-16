using Syinpo.Core.Caches;
using Syinpo.Core.Domain.Poco;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.BusinessLogic.Caches.Subscribes
{
    public class UserCacheSubscribe : ICacheSubscribe<User>
    {
        private readonly ICache _cache;

        public UserCacheSubscribe(ICache cache)
        {
            _cache = cache;
        }

        public void Handle(User entity, ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Delete:
                    {
                        _cache.HRemove(CacheKeys.User_Hash, string.Format(CacheKeys.User_Hash_Id, entity.Id));
                        _cache.HRemove(CacheKeys.User_Hash, string.Format(CacheKeys.User_Hash_Name, entity.Username));
                        break;
                    }
                case ChangeType.Insert:
                    {

                        break;
                    }
                case ChangeType.Update:
                    {
                        _cache.HRemove(CacheKeys.User_Hash, string.Format(CacheKeys.User_Hash_Id, entity.Id));
                        _cache.HRemove(CacheKeys.User_Hash, string.Format(CacheKeys.User_Hash_Name, entity.Username));
                        break;
                    }
            }
        }
    }
}

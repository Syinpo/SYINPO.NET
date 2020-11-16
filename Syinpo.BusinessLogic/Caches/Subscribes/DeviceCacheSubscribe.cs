using Syinpo.Core.Caches;
using Syinpo.Core.Domain.Poco;

namespace Syinpo.BusinessLogic.Caches.Subscribes {
    public class DeviceCacheSubscribe : ICacheSubscribe<Device> {
        private readonly ICache _cache;

        public DeviceCacheSubscribe( ICache cache )
        {
            _cache = cache;
        }

        public void Handle( Device entity, ChangeType changeType ) {
            switch( changeType ) {
                case ChangeType.Delete:
                {
                    _cache.HRemove(CacheKeys.Device_Hash, string.Format(CacheKeys.Device_Hash_Id, entity.Id));
                    _cache.HRemove( CacheKeys.Device_Hash, string.Format( CacheKeys.Device_Hash_Uuid, entity.DeviceUuid ) );

                    _cache.HRemove( CacheKeys.Device_WeiXin_Hash, string.Format( CacheKeys.Device_Hash_Id, entity.Id ) );
                        break;
                }
                case ChangeType.Insert: {

                    break;
                }
                case ChangeType.Update: {
                    _cache.HRemove( CacheKeys.Device_Hash, string.Format( CacheKeys.Device_Hash_Id, entity.Id ) );
                    _cache.HRemove( CacheKeys.Device_Hash, string.Format( CacheKeys.Device_Hash_Uuid, entity.DeviceUuid ) );

                    _cache.HRemove( CacheKeys.Device_WeiXin_Hash, string.Format( CacheKeys.Device_Hash_Id, entity.Id ) );
                        break;
                }
            }
        }
    }
}

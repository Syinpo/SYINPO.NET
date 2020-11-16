using Syinpo.Core.Caches;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Syinpo.BusinessLogic.Caches;
using Syinpo.Core.Helpers;

namespace Syinpo.BusinessLogic.SignalR.Online {
    public class OnlineRedisManager : IOnlineManager {


        private readonly ICache _cache;

        public OnlineRedisManager( ICache cache ) {
            _cache = cache;
        }

        public virtual void Add( OnlineClient client ) {
            AsyncHelper.RunSync( () => _cache.HSet( CacheKeys.Online_Hash,
                 string.Format( CacheKeys.Online_Hash_Id, client.ConnectionId ), client ) );

            if( client.UserId.HasValue ) {
                AsyncHelper.RunSync( () => _cache.HSet( CacheKeys.Online_User_Hash,
                    string.Format( CacheKeys.Online_Hash_Id, client.ConnectionId ), client ) );
            }
        }

        public virtual bool Remove( string connectionId ) {
            AsyncHelper.RunSync( () => _cache.HRemove(
                 CacheKeys.Online_Hash,
                 string.Format( CacheKeys.Online_Hash_Id, connectionId ) ) );


            AsyncHelper.RunSync( () => _cache.HRemove(
                CacheKeys.Online_User_Hash,
                string.Format( CacheKeys.Online_Hash_Id, connectionId ) ) );

            return true;
        }

        public virtual bool Remove( OnlineClient client )
        {
            throw new System.Exception("没有实现");

            AsyncHelper.RunSync( () => _cache.HRemove( CacheKeys.Online_Hash,
                 string.Format( CacheKeys.Online_Hash_Id, client.ConnectionId ) ) );
            return true;
        }

        public virtual OnlineClient GetByConnectionIdOrNull( string connectionId ) {
            throw new System.Exception( "没有实现" );

            return AsyncHelper.RunSync( () => _cache.HGet<OnlineClient>( CacheKeys.Online_Hash,
                 string.Format( CacheKeys.Online_Hash_Id, connectionId ) ) );
        }

        public virtual IReadOnlyList<OnlineClient> GetAllClients() {
            return AsyncHelper.RunSync( () => _cache.HSearch<OnlineClient>( CacheKeys.Online_Hash ) );
        }


        public virtual IReadOnlyList<OnlineClient> GetAllUsers() {
            return AsyncHelper.RunSync( () => _cache.HSearch<OnlineClient>( CacheKeys.Online_User_Hash ) );
        }

        public virtual long GetUserOnlinesCount() {
            return AsyncHelper.RunSync( () => _cache.HCount<OnlineClient>( CacheKeys.Online_User_Hash ) );
        }


        public virtual IReadOnlyList<OnlineClient> GetAllByUserId( int userId ) {
            return GetAllUsers().Where( c => c.UserId == userId ).ToImmutableList();
        }

        public virtual bool IsOnline( int userId ) {
            return GetAllByUserId( userId ).Any();
        }

        public virtual IReadOnlyList<OnlineClient> GetAllByDeviceId( int deviceId ) {
            return GetAllClients().Where( c => c.DeviceId == deviceId ).ToImmutableList();
        }

        public virtual bool IsDeviceOnline( int deviceId ) {
            return GetAllByDeviceId( deviceId ).Any();
        }


    }
}

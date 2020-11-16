using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Syinpo.BusinessLogic.SignalR.Online {
    public class OnlineManager : IOnlineManager {
        /// <summary>
        /// 在线客户端
        /// </summary>
        protected ConcurrentDictionary<string, OnlineClient> Clients {
            get;
        }

        /// <summary>
        /// 并发锁
        /// </summary>
        protected readonly object SyncObj = new object();

        public OnlineManager() {
            Clients = new ConcurrentDictionary<string, OnlineClient>();
        }


        public virtual void Add( OnlineClient client ) {
            lock( SyncObj ) {
                var userAlreadyExists = false;
                var userId = client.UserId;

                //if( userId != null ) {
                //    userAlreadyExists = this.IsOnline( userId.Value );
                //}

                Clients[ client.ConnectionId ] = client;
                //Clients.AddOrUpdate( client.ConnectionId, client, ( key, value ) => client );

                if( userId != null && !userAlreadyExists ) {
                }
            }
        }

        public virtual bool Remove( string connectionId ) {
            lock( SyncObj ) {
                var isRemoved = Clients.TryRemove( connectionId, out var client );

                if( isRemoved ) {
                    var userId = client.UserId;

                    if( userId != null && !this.IsOnline( userId.Value ) ) {
                    }
                }

                return isRemoved;
            }
        }

        public virtual bool Remove( OnlineClient client ) {
            return Remove( client.ConnectionId );
        }

        public virtual OnlineClient GetByConnectionIdOrNull( string connectionId ) {
            lock( SyncObj ) {
                return Clients.TryGetValue( connectionId, out var obj ) ? obj : default( OnlineClient );
            }
        }

        public virtual IReadOnlyList<OnlineClient> GetAllClients() {
            lock( SyncObj ) {
                return Clients.Values.ToImmutableList
                    ();
            }
        }

        public virtual IReadOnlyList<OnlineClient> GetAllUsers() {
            return GetAllClients().Where( c => c.UserId.HasValue ).ToImmutableList();
        }

        public virtual long GetUserOnlinesCount() {
            return (long)GetAllUsers().Count;
        }

        public virtual IReadOnlyList<OnlineClient> GetAllByUserId( int userId ) {
            return GetAllClients().Where( c => c.UserId == userId ).ToImmutableList();
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

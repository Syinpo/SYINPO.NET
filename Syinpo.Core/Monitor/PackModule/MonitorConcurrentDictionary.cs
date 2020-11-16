using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Syinpo.Core.Monitor.PackModule {

    public class MonitorConcurrentDictionary<T> {
        readonly ConcurrentDictionary<string, List<T>> Clients = new ConcurrentDictionary<string, List<T>>();

        /// <summary>
        /// 并发锁
        /// </summary>
        protected readonly object SyncObj = new object();

        public MonitorConcurrentDictionary() {
        }

        public int Count => Clients.Count;

        private void call( string key, T item ) {
            if( Clients.ContainsKey( key ) )
                Clients[ key ].Add( item );
            else {
                Clients.TryAdd( key, new List<T> { item } );
            }
        }


        public void Add( string key, T log ) {
            lock( SyncObj ) {
                call( key, log );
            }
        }

        public void AddRang( string timeGroup, List<T> data ) {
            lock( SyncObj ) {
                if( Clients.ContainsKey( timeGroup ) )
                    Clients[ timeGroup ].AddRange( data );
                else {
                    Clients.TryAdd( timeGroup, data );
                }
            }
        }

        public bool Remove( string key ) {
            lock( SyncObj ) {
                var isRemoved = Clients.TryRemove( key, out var client );

                if( isRemoved ) {
                }

                return isRemoved;
            }
        }

        public List<T> GetByKey( string key ) {
            lock( SyncObj ) {
                Clients.TryGetValue( key, out List<T> logs );
                if (logs == null)
                    return new List<T>();

                return logs;
            }
        }

        public List<string> GetKeys() {
            lock( SyncObj ) {
                return Clients.Keys.ToList();
            }
        }
    }
}

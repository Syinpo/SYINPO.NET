using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Syinpo.Core.Caches {
    public static class CacheExtension {

        private static readonly ConcurrentDictionary<string, object> _getSetNullLocks = new ConcurrentDictionary<string, object>();
        public static async Task<T> GetSetObj<T>( this IDistributedCache cache, string key, Func<Task<T>> lookup, TimeSpan? duration = null ) {
            var data = await cache.GetStringAsync( key );
            if( !string.IsNullOrEmpty( data ) ) {
                var item = JsonConvert.DeserializeObject<T>( data );
                if( item == null )
                    return default( T );

                return item;
            }

            T result;
            var localLockName = key;
            var nullLoadLock = _getSetNullLocks.AddOrUpdate( localLockName, k => new object(), ( k, old ) => old );
            lock( nullLoadLock ) {
                var task = lookup.Invoke();
                if( task != null ) {
                    result = task.Result;
                }
                else
                {
                    result = default(T);
                }
            }

            _getSetNullLocks.TryRemove(localLockName, out object old2);
            cache.Remove( key );

            if( result != null ) {
                if( duration.HasValue ) {
                    await cache.SetObj( key, result, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = duration } );
                }
                else {
                    await cache.SetObj( key, result );
                }
            }

            return result;
        }

        public static async Task<T> GetObj<T>( this IDistributedCache cache, string key ) {
            var data = await cache.GetStringAsync( key );
            if( string.IsNullOrEmpty( data ) )
                return default( T );

            var item = JsonConvert.DeserializeObject<T>( data );
            if( item == null )
                return default( T );

            return item;
        }

        public static async Task SetObj( this IDistributedCache cache, string key, object data, DistributedCacheEntryOptions option = null ) {
            if( data == null )
                return;

            if( option == null )
                option = new DistributedCacheEntryOptions();

            var item = JsonConvert.SerializeObject( data );
            await cache.SetStringAsync( key, item, option );
        }

        public static async Task<bool> Exist( this IDistributedCache cache, string key ) {
            var cached = await cache.GetAsync( key );

            return cached != null;
        }
    }
}

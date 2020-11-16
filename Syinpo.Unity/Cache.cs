using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachingFramework.Redis;
using Syinpo.BusinessLogic.Caches;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Unity.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Syinpo.Unity {
    public class Cache : ICache {
        #region 字段

        private readonly IDistributedCache _cache;
        private readonly IRedisCacheWrapper _redisCacheWrapper;
        private readonly CacheOptions _options;

        #endregion

        #region 构造函数

        public Cache( IDistributedCache cache, IRedisCacheWrapper redisCacheWrapper, IOptions<CacheOptions> cacheOptions ) {
            _cache = cache;
            _redisCacheWrapper = redisCacheWrapper;
            _options = cacheOptions.Value;
        }

        #endregion

        #region Distribute String

        public async Task<T> GetSet<T>( string key, Func<Task<T>> lookup, int cacheMinutes = 0 ) {
            if( !_options.EnableCache ) {
                var task = lookup.Invoke();
                if( task != null ) {
                    return await task;
                }

                return default( T );
            }


            if( cacheMinutes > 0 )
                return await _cache.GetSetObj( key, lookup, TimeSpan.FromMinutes( cacheMinutes ) );

            return await _cache.GetSetObj( key, lookup );
        }

        public async Task<T> Get<T>( string key ) {
            return await _cache.GetObj<T>( key );
        }

        public async Task Set( string key, object value, int cacheMinutes = 0 ) {
            if( cacheMinutes > 0 ) {
                await _cache.SetObj( key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes( cacheMinutes ) } );
            }
            else {
                await _cache.SetObj( key, value );
            }
        }

        #endregion

        #region Distribute Keys

        public async Task<bool> Exist( string key ) {
            return await _cache.Exist( key );
        }

        public async Task Remove( string key ) {
            await _cache.RemoveAsync( key );
        }


        #endregion

        #region Redis Hash

        public async Task<T> HGetSet<T>( string hash, string key, Func<Task<T>> lookup, int cacheMinutes = 0 ) {
            if( !_options.EnableCache ) {
                var task = lookup.Invoke();
                if( task != null ) {
                    return await task;
                }

                return default( T );
            }

            T result;

            var context = await _redisCacheWrapper.RedisContext();

            if( cacheMinutes > 0 )
                result = await context.Cache.FetchHashedAsync<T>( hash, key, lookup, TimeSpan.FromMinutes( cacheMinutes ) );
            else {
                result = await context.Cache.FetchHashedAsync<T>( hash, key, lookup );
            }

            return result;
        }

        public async Task HSet( string hash, string key, object value, int cacheMinutes = 0 ) {
            var context = await _redisCacheWrapper.RedisContext();

            if( cacheMinutes > 0 )
                await context.Cache.SetHashedAsync( hash, key, value, TimeSpan.FromMinutes( cacheMinutes ) );
            else
                await context.Cache.SetHashedAsync( hash, key, value );
        }

        public async Task HSetRange<T>( string hash, Dictionary<string, T> keyValCollection ) {
            var context = await _redisCacheWrapper.RedisContext();
            await context.Collections.GetRedisDictionary<string, T>( hash ).AddRangeAsync( keyValCollection );
        }

        public async Task<long> HCount<T>( string hash ) {
            var context = await _redisCacheWrapper.RedisContext();
           return await context.Collections.GetRedisDictionary<string, T>( hash ).GetCountAsync();
        }

        public async Task<T> HGet<T>( string hash, string key ) {
            //var data = await _redisCacheWrapper.RedisContext.Collections.GetRedisDictionary<string, T>( hash )
            //    .TryGetValueAsync( key );
            //return data.Value;
            var context = await _redisCacheWrapper.RedisContext();
            return await context.Cache.GetHashedAsync<T>( hash, key );
        }

        /// <summary>
        /// HSearch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash"></param>
        /// <param name="keys"></param>
        /// <param name="likePattern"> "user:*", "*" </param>
        /// <returns></returns>
        public async Task<List<T>> HSearch<T>( string hash, string[] keys = null, string likePattern = null ) {
            var result = new List<T>();
            if( string.IsNullOrEmpty( hash ) )
                return result;

            var context = await _redisCacheWrapper.RedisContext();
            if( keys == null && string.IsNullOrEmpty( likePattern ) ) {
                var data = context.Cache.ScanHashed<T>( hash, "*", 99999 ).Select( s => s.Value ).ToList();
                result.AddRange( data );

                return result;
            }

            if( keys != null && keys.Length > 0 ) {
                var data = await context.Cache.GetHashedAsync<string, T>( hash, keys );
                result.AddRange( data.ToList() );
            }

            if( !string.IsNullOrEmpty( likePattern ) ) {
                var data = context.Cache.ScanHashed<T>( hash, likePattern ).Select( s => s.Value ).ToList();
                result.AddRange( data );
            }

            return result;
        }

        public async Task HRemove( string hash, string key ) {
            if( !_options.EnableCache )
                return;

            var context = await _redisCacheWrapper.RedisContext();
            await context.Cache.RemoveHashedAsync( hash, key );
        }

        #endregion

        #region Redis List

        public async Task LSet<T>( string key, T value ) {
            var context = await _redisCacheWrapper.RedisContext();
            var rl = context.Collections.GetRedisList<T>( key );

            await rl.PushLastAsync( value );
        }

        public async Task LSetRange<T>( string key, List<T> list ) {
            var context = await _redisCacheWrapper.RedisContext();
            var rl = context.Collections.GetRedisList<T>( key );

            await rl.AddRangeAsync( list );
        }

        public async Task<List<T>> LSearch<T>( string key, int page = 1, int pageSize = 100 ) {
            if( page <= 0 ) {
                page = 1;
            }

            var context = await _redisCacheWrapper.RedisContext();
            var rl = context.Collections.GetRedisList<T>( key );

            var start = ( page - 1 ) * pageSize;
            var stop = pageSize;

            return ( await rl.GetRangeAsync( start, stop ) ).ToList();
        }

        public async Task LRemove<T>( string key, T item ) {
            var context = await _redisCacheWrapper.RedisContext();
            var rl = context.Collections.GetRedisList<T>( key );

            await rl.RemoveAsync( item, 0 );
        }

        public async Task LRemoveIndex<T>( string key, int page = 1, int pageSize = 100 ) {
            if( page <= 0 ) {
                page = 1;
            }

            var context = await _redisCacheWrapper.RedisContext();
            var rl = context.Collections.GetRedisList<T>( key );

            if( page == 1 ) {
                var start = pageSize;
                var stop = 0;

                await rl.TrimAsync( start, stop );
            }
            else {
                var start = ( page - 1 ) * pageSize;
                var stop = pageSize;

                for( int i = start; i <= stop; i++ ) {
                    await rl.RemoveAtAsync( i );
                }
            }
        }

        public async Task LRemoveList<T>( string key, List<T> items ) {
            var context = await _redisCacheWrapper.RedisContext();
            var rl = context.Collections.GetRedisList<T>( key );

            foreach( var item in items ) {
                await rl.RemoveAsync( item, 0 );
            }
        }

        #endregion

        #region Redis Context

        public RedisContext Current => _redisCacheWrapper.RedisContext().ConfigureAwait( false ).GetAwaiter().GetResult();

        #endregion

        #region Redis Fush

        public async Task Clear() {

        }

        #endregion

        #region Lock

        public async Task<bool> LockWith( string name, TimeSpan exp, Action action ) {
            var redisLockFactory = await _redisCacheWrapper.GetRedisLockFactory();
            using var redisLock = await redisLockFactory.CreateLockAsync(
                name, exp,
                TimeSpan.FromSeconds( 5 ),
                TimeSpan.FromSeconds( 1 ) );
            if( !redisLock.IsAcquired )
                return false;

            action();

            return true;
        }

        public async Task LockWith(string name, TimeSpan expiry, TimeSpan wait, TimeSpan retry, Action action)
        {
            var redisLockFactory = await _redisCacheWrapper.GetRedisLockFactory();
            using var redisLock = await redisLockFactory.CreateLockAsync(
                name, 
                expiry,
                wait,
                retry);
            if (redisLock.IsAcquired)
            {
                action();
            }

        }

        #endregion
    }

}

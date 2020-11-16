using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CachingFramework.Redis;

namespace Syinpo.Core.Caches {
    public interface ICache {
        Task<T> GetSet<T>( string key, Func<Task<T>> lookup, int cacheMinutes = 0 );
        Task<T> Get<T>( string key );
        Task Set( string key, object value, int cacheMinutes = 0 );
        Task Remove( string key );
        Task Clear();

        #region Redis Hash

        Task<T> HGetSet<T>( string hash, string key, Func<Task<T>> lookup, int cacheMinutes = 0 );

        Task HSet( string hash, string key, object value, int cacheMinutes = 0 );

        Task HSetRange<T>( string hash, Dictionary<string, T> keyValCollection );

        Task<long> HCount<T>(string hash);

        Task<T> HGet<T>( string hash, string key );

        /// <summary>
        /// HSearch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash"></param>
        /// <param name="keys"></param>
        /// <param name="likePattern"> "user:*", "*" </param>
        /// <returns></returns>
        Task<List<T>> HSearch<T>( string hash, string[] keys = null, string likePattern = null );

        Task HRemove( string hash, string key );

        RedisContext Current {
            get;
        }

        #endregion

        #region Redis List

        Task LSet<T>( string key, T value );

        Task LSetRange<T>( string key, List<T> list );

        Task<List<T>> LSearch<T>( string key, int page = 1, int pageSize = 100 );

        Task LRemove<T>( string key, T item );

        Task LRemoveIndex<T>(string key, int page = 1, int pageSize = 100);

        Task LRemoveList<T>(string key, List<T> items);


        #endregion

        Task<bool> LockWith( string name, TimeSpan exp, Action action );
        Task LockWith(string name, TimeSpan expiry, TimeSpan wait, TimeSpan retry, Action action);
    }
}
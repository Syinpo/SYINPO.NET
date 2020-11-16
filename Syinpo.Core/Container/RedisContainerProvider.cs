using Syinpo.Core.Caches;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Syinpo.Core.Container {
    public class RedisContainerProvider : IDataContainer {
        private readonly ICache _cache;

        public RedisContainerProvider( ICache cache ) {
            _cache = cache;

        }

        public async Task Publish<T>( string catetory, T item ) {
            await _cache.LSet( catetory, item );
        }

        public async Task<List<T>> GetBatch<T>( string catetory, int page = 1, int pageSize = 100 ) {
            return await _cache.LSearch<T>( catetory, page, pageSize );
        }

        public async Task RemoveIndex<T>( string catetory, int page = 1, int pageSize = 100 ) {
            await _cache.LRemoveIndex<T>( catetory, page, pageSize );
        }

        public async Task RemoveList<T>( string catetory, List<T> items ) {
            await _cache.LRemoveList<T>( catetory, items );
        }
    }
}

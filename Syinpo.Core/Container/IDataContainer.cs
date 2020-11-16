using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Syinpo.Core.Container {
    public interface IDataContainer {
        Task Publish<T>( string catetory, T item );

        Task<List<T>> GetBatch<T>( string catetory, int page = 1, int pageSize = 100 );

        Task RemoveIndex<T>( string catetory, int page = 1, int pageSize = 100 );

        Task RemoveList<T>( string catetory, List<T> items );
    }
}

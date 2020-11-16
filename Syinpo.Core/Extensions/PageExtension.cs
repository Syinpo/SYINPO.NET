using Syinpo.Core.Mapper;
using System;
using System.Linq;

namespace Syinpo.Core.Extensions {
    public static class PageExtension {
        public static PageList<T> ToPageList<T>( this IQueryable<T> query, int pageIndex, int pageSize ) where T : class {
            return new PageList<T>( query, pageIndex, pageSize );
        }

        public static PageResult<T> ToPageResult<T>( this PageList<T> source ) where T : class {
            return new PageResult<T>( source );
        }

        public static PageResult<TU> ToPageResult<T, TU>( this PageList<T> source ) where T : class {
            var result = new PageResult<TU>();
            result.Page = source.PageIndex + 1;
            result.PageSize = source.PageSize;
            result.TotalCount = source.TotalCount;
            result.TotalPages = source.TotalPages;
            result.Results = source.MapToList<TU>();
            return result;
        }
    }
}

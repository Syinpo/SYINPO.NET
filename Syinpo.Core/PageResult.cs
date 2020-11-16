using System.Collections.Generic;
using System.Linq;

namespace Syinpo.Core {
    public class PageResult<T> {
        public PageResult( PageList<T> source ) {
            Results = new List<T>();
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;
            this.PageSize = source.PageSize;
            this.Page = source.PageIndex + 1;
            this.Results.AddRange( source );
        }

        public List<T> Results {
            get; set;
        }
        public int Page {
            get; set;
        }
        public int PageSize {
            get; set;
        }
        public int TotalCount {
            get; set;
        }
        public int TotalPages {
            get; set;
        }

        public PageResult() {
            Results = new List<T>();
        }

        public bool HasPreviousPage {
            get {
                return ( ( Page - 1 ) > 0 );
            }
        }
        public bool HasNextPage {
            get {
                return ( ( Page - 1 ) + 1 < TotalPages );
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Data {
    public interface IDatabaseHelper {
        bool AccorMainDb {
            get; set;
        }

        bool NotCacheable {
            get; set;
        }

    }
}

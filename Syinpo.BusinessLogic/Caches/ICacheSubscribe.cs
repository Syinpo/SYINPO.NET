using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.BusinessLogic.Caches {
    public interface ICacheSubscribe<T>
    {
        void Handle( T entity, ChangeType changeType );
    }
}

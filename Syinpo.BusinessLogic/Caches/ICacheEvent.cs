using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.BusinessLogic.Caches {
   public interface ICacheEvent
   {
       void Publish<T>(T entity, ChangeType changeType);
   }
}

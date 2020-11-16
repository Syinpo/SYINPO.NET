using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Caches
{
    public interface IMd5Cache
    {
        int Count { get; }
        bool IsEmpty { get; }
        int MaxCount { get; set; }

        bool Contains(string md5);
        void Enqueue(string md5);
        bool TryDequeue(out string result);
    }
}

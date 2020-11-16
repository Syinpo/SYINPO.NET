using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syinpo.Core.Caches
{
    public class Md5Cache : IMd5Cache
    {
        private readonly ConcurrentQueue<string> _md5Cache = new ConcurrentQueue<string>();

        public Md5Cache( int maxCount = 256 )
        {
            MaxCount = maxCount;
        }


        public int Count
        {
            get
            {
                return _md5Cache.Count;
            }
        }

        public int MaxCount { get; set; } = 256;

        public bool IsEmpty
        {
            get
            {
                return _md5Cache.IsEmpty;
            }
        }

        public bool Contains(string md5)
        {
            return !_md5Cache.IsEmpty && _md5Cache.Contains(md5);
        }

        public void Enqueue(string md5)
        {
            _md5Cache.Enqueue(md5);

            if (_md5Cache.Count > MaxCount)
            {
                _md5Cache.TryDequeue(out string result);
            }
        }

        public bool TryDequeue(out string result)
        {
            return _md5Cache.TryDequeue(out result);
        }

    }
}

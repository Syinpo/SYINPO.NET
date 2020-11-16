using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Caches {
    public class CacheOptions {
        public bool EnableCache {get; set;} = true;

        public string UseCache { get; set; } = "Memory";

        public string RedisConfiguration { get; set; } = "localhost:6379";

        public string RedisInstanceName { get; set; } = "SyinpoInstance";
    }
}

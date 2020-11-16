using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Syinpo.Core.SignalBus.Model;
using Microsoft.Extensions.Caching.Memory;
using OpenTelemetry.Metrics;

namespace Syinpo.Unity.Firewall.Store {
    public class ThrottlerMemoryStore {
        public static ConcurrentDictionary<string, ThrottlerItem> _cache = new ConcurrentDictionary<string, ThrottlerItem>();


        public static ThrottlerItem Get( ThrottlerItem throttlerItem ) {
            if( throttlerItem == null )
                return default( ThrottlerItem );


            var key = throttlerItem.GenerateKey();
            if( _cache.TryGetValue( key, out ThrottlerItem stored ) ) {
                return stored;
            }

            return throttlerItem;
        }

        public static void Remove( ThrottlerItem throttlerItem ) {
            var key = throttlerItem.GenerateKey();
            _cache.TryRemove( key, out ThrottlerItem stored );
        }

        public static void Set( ThrottlerItem throttlerItem ) {
            var key = throttlerItem.GenerateKey();
            _cache.AddOrUpdate( key, throttlerItem, ( key, oldValue ) => oldValue = throttlerItem );
        }


    }
}

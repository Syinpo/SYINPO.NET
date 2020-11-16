using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Unity.Firewall.Store {
    public class RouteMemoryStore {
        public static ConcurrentDictionary<string, RouteItem> _cache = new ConcurrentDictionary<string, RouteItem>();


        public static RouteItem Get( string route ) {
            if( string.IsNullOrEmpty( route ) )
                return default( RouteItem );

            if( !_cache.ContainsKey( route ) )
                return default( RouteItem );


            if( _cache.TryGetValue( route, out RouteItem stored ) ) {
                return stored;
            }

            return default( RouteItem );
        }

        public static void Remove( string route ) {
            _cache.TryRemove( route, out RouteItem stored );
        }

        public static void Set( RouteItem item ) {
            _cache.AddOrUpdate( item.Route, item, ( key, oldValue ) => oldValue = item );
        }


    }
}

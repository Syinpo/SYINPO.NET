using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Syinpo.Core.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Syinpo.Core.Data {
    public class DatabaseHelper : IDatabaseHelper {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICache _cache;
        private readonly ILogger<DatabaseHelper> _logger;

        public DatabaseHelper( IHttpContextAccessor httpContextAccessor, ICache cache, ILogger<DatabaseHelper> logger ) {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _logger = logger;
        }

        public bool AccorMainDb {
            get {
                if( _httpContextAccessor == null || _httpContextAccessor.HttpContext == null ) {
                    return true;
                }

                var d = _httpContextAccessor?.HttpContext?.Items[ "AccorMainDb" ];
                if( d == null )
                    return false;

                return (bool)d;
            }
            set {
                if( _httpContextAccessor == null || _httpContextAccessor.HttpContext == null ) {

                }
                else {
                    if( _httpContextAccessor?.HttpContext?.Items.ContainsKey( "AccorMainDb" ) == true )
                        _httpContextAccessor.HttpContext.Items[ "AccorMainDb" ] = value;
                    else
                        _httpContextAccessor?.HttpContext?.Items.Add( "AccorMainDb", value );
                }
            }
        }

        public bool NotCacheable {
            get {
                if( _httpContextAccessor == null || _httpContextAccessor.HttpContext == null ) {
                    return true;
                }

                var d = _httpContextAccessor?.HttpContext?.Items[ "NotCacheable" ];
                if( d == null )
                    return false;

                return (bool)d;
            }
            set {
                if( _httpContextAccessor == null || _httpContextAccessor.HttpContext == null ) {

                }
                else {
                    if( _httpContextAccessor?.HttpContext?.Items.ContainsKey( "NotCacheable" ) == true )
                        _httpContextAccessor.HttpContext.Items[ "NotCacheable" ] = value;
                    else
                        _httpContextAccessor?.HttpContext?.Items.Add( "NotCacheable", value );
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Core;
using Microsoft.Extensions.Logging;

namespace Syinpo.BusinessLogic.Caches {
    public class CacheEvent : ICacheEvent {
        private readonly ILogger<CacheEvent> _logger;
        public CacheEvent( ILogger<CacheEvent> logger ) {
            _logger = logger;
        }

        public void Publish<T>( T entity, ChangeType changeType ) {
            var cacheSubscribe = IoC.Resolve<ICacheSubscribe<T>>();
            if( cacheSubscribe == null )
                _logger.LogWarning( "CacheSubscribe: " + nameof( T ) + "没有实现" );

            try {
                cacheSubscribe.Handle( entity, changeType );
            }
            catch( Exception ex ) {

                _logger.LogError( "CacheEvent: " + ex.Message );
            }
        }

        public void BathPublish<T>( List<T> entities, ChangeType changeType, Func<T, object> keySelector ) {
            var cacheSubscribe = IoC.Resolve<ICacheSubscribe<T>>();
            if( cacheSubscribe == null )
                _logger.LogWarning( "CacheSubscribe: " + nameof( T ) + "没有实现" );

            try {
                foreach( var item in entities ) {
                    cacheSubscribe.Handle( item, changeType );
                }
            }
            catch( Exception ex ) {

                _logger.LogError( "CacheEvent BathPublish: " + ex.Message );
            }
        }
    }
}

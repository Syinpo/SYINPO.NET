using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.MiddlewareResolver;

namespace Syinpo.Unity.Autofac {
    public class PipelineNetMiddlewareResolver : IMiddlewareResolver {
        private readonly IServiceProvider _serviceProvider;

        public PipelineNetMiddlewareResolver( IServiceProvider serviceProvider ) {
            _serviceProvider = serviceProvider;
        }

        protected   IServiceProvider GetServiceProvider() {
            if( _serviceProvider == null )
                return null;

            var accessor = _serviceProvider.GetService<IHttpContextAccessor>();
            if( accessor != null ) {
                var context = accessor.HttpContext;

                if( context?.RequestServices != null ) {
                    return context.RequestServices;
                }
            }

            return _serviceProvider;
        }

        public   object Resolve( Type type ) {
            return GetServiceProvider().GetRequiredService( type );
        }
    }
}

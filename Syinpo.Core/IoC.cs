using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Syinpo.Core {
    public class IoC {
        private static IServiceProvider ServiceProvider {
            get; set;
        }

        private static IContainer Container {
            get; set;
        }

        static object lockOject = new object();
        public static void Init( IServiceProvider serviceProvider, IContainer container ) {
            lock( lockOject ) {
                if( ServiceProvider == null )
                    ServiceProvider = serviceProvider;

                if( Container == null )
                    Container = container;
            }
        }

        protected static IServiceProvider GetServiceProvider() {
            if( ServiceProvider == null )
                return null;

            var accessor = ServiceProvider.GetService<IHttpContextAccessor>();
            if( accessor != null ) {
                var context = accessor.HttpContext;

                if( context?.RequestServices != null ) {
                    return context.RequestServices;
                }
            }

            return ServiceProvider;
        }

        public static T Resolve<T>() {
            if( GetServiceProvider() == null )
                return default( T );

            return (T)GetServiceProvider().GetRequiredService( typeof( T ) );
        }

        public static object Resolve( Type type ) {
            return GetServiceProvider().GetRequiredService( type );
        }

        public static T Resolve<T>( string named ) {
            if( Container == null )
                return default( T );

            return Container.ResolveNamed<T>( named );
        }
    }
}

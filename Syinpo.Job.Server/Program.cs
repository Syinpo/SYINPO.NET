using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Syinpo.Job.Server {
    public class Program {
        public static void Main( string[] args ) {
            CreateWebHostBuilder( args ).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder( string[] args ) =>
            WebHost.CreateDefaultBuilder( args )
                .ConfigureAppConfiguration( ( hostingContext, config ) =>
                {
                    if( hostingContext.HostingEnvironment.IsDevelopment() )
                    {
                        config.AddJsonFile( Path.Combine( AppContext.BaseDirectory ,"appsettings.json" ) );
                    }
                } )
                .UseStartup<Startup>();
    }
}

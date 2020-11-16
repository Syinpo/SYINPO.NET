using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Syinpo.Monitor.Api {

    public class Program {

        //public static void Main( string[] args ) {
        //    CreateHostBuilder( args ).Build().Run();
        //}

        //public static IHostBuilder CreateHostBuilder( string[] args ) =>
        //    Host.CreateDefaultBuilder( args )
        //        .UseServiceProviderFactory( new AutofacServiceProviderFactory() )
        //        .ConfigureWebHostDefaults( webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        } );

        public static void Main( string[] args ) {
            CreateWebHostBuilder( args ).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder( string[] args ) =>
            WebHost.CreateDefaultBuilder( args )
                .ConfigureAppConfiguration( ( hostingContext, config ) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        config.Sources.Clear();

                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                                optional: true, reloadOnChange: true)
                            .AddJsonFile( Path.Combine( env.ContentRootPath, @"bin\Debug\netcoreapp3.1", "appsettings.json" ),
                                optional: true, reloadOnChange: true );

                        config.AddEnvironmentVariables();

                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }
                    }
                } )
                .UseStartup<Startup>();
    }
}
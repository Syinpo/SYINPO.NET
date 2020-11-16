using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Syinpo.Update.Api {

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
                .UseStartup<Startup>();
    }
}
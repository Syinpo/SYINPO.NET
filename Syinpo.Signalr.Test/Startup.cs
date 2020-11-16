using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Syinpo.Signalr.Test {
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services ) {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env ) {
            if( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            var tokenClient = new HttpClient();
            //var tokenClient = new TokenClient( "http://192.168.50.58:6000/connect/token", "client", "secret" );
            //var tokenResponse = tokenClient.RequestPasswordTokenAsync( "admin", "123456", "api" ).Result;
            var tokenResponse = tokenClient.RequestPasswordTokenAsync( new PasswordTokenRequest {
                Address = "https://localhost:44372/connect/token",

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api",

                UserName = "admin",
                Password = "123456"
            } ).ConfigureAwait( false ).GetAwaiter().GetResult();

            var connection = new HubConnectionBuilder()
                .WithUrl( "https://localhost:44366/hubs/groupControlHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult( tokenResponse.AccessToken );
                } )
                .Build();


        }
    }
}

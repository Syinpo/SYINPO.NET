using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using FluentValidation.Attributes;
using IdentityServer4.AccessTokenValidation;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Helpers;
using Syinpo.Unity.AspNetCore.Authorization.Policies;
using Syinpo.Unity.AspNetCore.Authorization.Requirements;
using Syinpo.Unity.AspNetCore.Filters;
using Syinpo.Unity.AspNetCore.Middlewares;
using Syinpo.Unity.Autofac;
using Syinpo.Unity.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Syinpo.Monitor.Api {
    public class Startup {
        public Startup( IConfiguration configuration ) {
            Configuration = configuration;
        }

        public IConfiguration Configuration {
            get;
        }

        public IServiceProvider ConfigureServices( IServiceCollection services ) {
            services.AddMvc( c => c.EnableEndpointRouting = false );
            services.AddControllers( config =>
                 {
                     config.Filters.Add( typeof( CustomExceptionFilter ) );
                 } )
                .AddNewtonsoftJson()
                .AddControllersAsServices()
                .AddFluentValidation( cfg =>
                 {
                     cfg.ValidatorFactoryType = typeof( AttributedValidatorFactory );
                     cfg.ImplicitlyValidateChildProperties = true;
                 } );
            services.AddOptions();
            services.AddHttpClient( "monitor" );

            // https
            var useHttps = Configuration.GetValue<bool?>( "UseHttps" );
            if( useHttps.HasValue && useHttps.Value ) {
                services.AddHttpsRedirection( options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                } );
            }

            // log
            var seqServer = Configuration.GetValue<string>( "SeqServer" );
            var levelSwitch = new LoggingLevelSwitch( Serilog.Events.LogEventLevel.Information );
            if( string.IsNullOrEmpty( seqServer ) ) {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy( levelSwitch )
                    .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile( pathFormat: Path.Combine( AppContext.BaseDirectory, "logs\\log-{Date}.log" ) )
                    //.WriteTo.Stackify( restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug )
                    .CreateLogger();
            }
            else {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy( levelSwitch )
                    .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                    .Enrich.FromLogContext()
                    .WriteTo.Seq( seqServer )
                    //.WriteTo.Stackify( restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information )
                    .CreateLogger();
            }
            services.AddLogging( loggingBuilder => loggingBuilder.AddSerilog( dispose: true ) );

            //automapper
            services.AddAutoMapper( typeof( Startup ) );
            MapperRegister.Register();

            // 跨域
            services.AddCors( o => o.AddPolicy( "AllowAllPolicy", builder =>
              {
                  builder
                      .SetIsOriginAllowed( origin => true )
                      .WithMethods( "GET", "POST", "DELETE", "OPTIONS", "PUT" )
                      .AllowAnyHeader()
                      .AllowCredentials();
              } ) );

            // token
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();
            services.AddAuthentication();
            services.AddAuthentication( IdentityServerAuthenticationDefaults.AuthenticationScheme )
                .AddJwtBearer( options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters {
                         ValidateAudience = false,
                         ValidateIssuer = true,
                         ValidIssuer = Configuration.GetValue<string>( "IdentityServer" ),
                         ValidateIssuerSigningKey = false,
                         IssuerSigningKey = signingKey,
                         ValidateLifetime = false
                     };

                     // WebSocket的Token来自QueryString
                     options.Events = new JwtBearerEvents {
                         OnMessageReceived = context =>
                         {
                             var accessToken = context.Request.Query[ "access_token" ];
                             if( string.IsNullOrEmpty( accessToken ) ) {
                                 context.Request.Query.TryGetValue( "token", out accessToken );
                             }

                             // If the request is for our hub...
                             var path = context.HttpContext.Request.Path;
                             if( !string.IsNullOrEmpty( accessToken ) && ( path.StartsWithSegments( "/hubs/" ) ) ) {
                                 // Read the token out of the query string
                                 context.Token = accessToken;
                             }

                             return Task.CompletedTask;
                         }
                     };
                 } );


            // permission
            services.AddAuthorization( options =>
             {
                 options.AddPolicy( "Permission", policyBuilder =>
                 {
                     policyBuilder.Requirements.Add( new PermissionRequirement() );
                     policyBuilder.RequireAuthenticatedUser();
                 } );
             } );
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationPolicy>();

            // cache
            services.Configure<CacheOptions>( Configuration.GetSection( "Cache" ) );
            var cacheOptions = Configuration.GetSection("Cache").Get<CacheOptions>();
            services.AddStackExchangeRedisCache( options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
                options.InstanceName = cacheOptions.RedisInstanceName;
            } );
            services.AddDistributedMemoryCache();

            // IoC & DI
            services.AddAutofac();
            var iocProvider = IoCRegister.Register( services, Configuration );
            IoC.Init( iocProvider.Item1, iocProvider.Item2 );

            return iocProvider.Item1;
        }

        public void Configure( IApplicationBuilder app, IHostingEnvironment env ) {

            if( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            //errors
            app.UseException();

            app.UseRouting();
            app.UseCors( "AllowAllPolicy" );
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            var useHttps = Configuration.GetValue<bool?>( "UseHttps" );
            if( useHttps.HasValue && useHttps.Value ) {
                app.UseHttpsRedirection();
            }

            app.UseEndpoints( endpoints =>
             {
                 endpoints.MapControllers();
                 endpoints.MapControllerRoute( "default", "{controller=Home}/{action=Index}/{id?}" );
                 endpoints.MapDefaultControllerRoute();
             } );
        }
    }
}

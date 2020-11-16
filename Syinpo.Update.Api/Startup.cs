using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using FluentValidation.Attributes;
using Hangfire;
using Hangfire.SqlServer;
using IdentityServer4.AccessTokenValidation;
using Syinpo.BusinessLogic.Content;
using Syinpo.BusinessLogic.SignalR.Hubs;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.EasyLicense.License.Validator;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Helpers;
using Syinpo.Unity.AspNetCore.Authorization.Policies;
using Syinpo.Unity.AspNetCore.Authorization.Requirements;
using Syinpo.Unity.AspNetCore.Filters;
using Syinpo.Unity.AspNetCore.Middlewares;
using Syinpo.Unity.Autofac;
using Syinpo.Unity.AutoMapper;
using Syinpo.Unity.Hangfire;
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
using Swashbuckle.AspNetCore.SwaggerGen;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Syinpo.Update.Api {
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
                     //config.Filters.Add( new CustomAuthorizeFilter( new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build() ) );
                 } )
                .AddNewtonsoftJson(
                //options =>
                //    options.SerializerSettings.ContractResolver =
                //        new CamelCasePropertyNamesContractResolver()
                )
                .AddControllersAsServices()
                .AddFluentValidation( cfg =>
                 {
                     cfg.ValidatorFactoryType = typeof( AttributedValidatorFactory );
                     cfg.ImplicitlyValidateChildProperties = true;
                 } );
            services.AddOptions();

            // https
            var useHttps = Configuration.GetValue<bool?>( "UseHttps" );
            if( useHttps.HasValue && useHttps.Value ) {
                services.AddHttpsRedirection( options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                } );
            }

            // ef pro
            // HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

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

            //signalr
            services.AddSignalR( p =>
            {
                p.EnableDetailedErrors = true;
                p.ClientTimeoutInterval = TimeSpan.FromSeconds( 60 );
                p.HandshakeTimeout = TimeSpan.FromSeconds( 30 );
                p.KeepAliveInterval = TimeSpan.FromSeconds( 15 );
            } );

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

            //.AddIdentityServerAuthentication( options =>
            //{
            //    options.Authority = Configuration.GetValue<string>( "IdentityServer" );
            //    options.RequireHttpsMetadata = false;// 指定是否为HTTPS

            //    options.ApiName = "api";

            //    // WebSocket的Token来自QueryString
            //    options.Events = new JwtBearerEvents {
            //        OnMessageReceived = context =>
            //        {
            //            var accessToken = context.Request.Query[ "access_token" ];
            //            if( string.IsNullOrEmpty( accessToken ) ) {
            //                context.Request.Query.TryGetValue( "token", out accessToken );
            //            }

            //            // If the request is for our hub...
            //            var path = context.HttpContext.Request.Path;
            //            if( !string.IsNullOrEmpty( accessToken ) && ( path.StartsWithSegments( "/hubs/" ) ) ) {
            //                // Read the token out of the query string
            //                context.Token = accessToken;
            //            }
            //            return Task.CompletedTask;
            //        }
            //    };
            //} );

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

            // api doc
            services.AddSwaggerGen( c =>
             {
                 // https://localhost:44312/swagger/v1/swagger.json
                 c.SwaggerDoc( "v1",
                      new Microsoft.OpenApi.Models.OpenApiInfo {
                          Title = "烽客 API",
                          Version = "v1",
                          Description = "烽客 API 文档",
                      } );

                 c.EnableAnnotations();

                 //c.CustomOperationIds( e => $"{e.ActionDescriptor.RouteValues[ "action" ]}");
                 c.CustomOperationIds( apiDesc =>
                  {
                      return apiDesc.TryGetMethodInfo( out MethodInfo methodInfo ) ? methodInfo.Name : null;
                  } );

                 // Set the comments path for the Swagger JSON and UI.
                 var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                 var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
                 if( File.Exists( xmlPath ) )
                     c.IncludeXmlComments( xmlPath );
                 var xmlPath2 = Path.Combine( AppContext.BaseDirectory, "Syinpo.Model.xml" );
                 if( File.Exists( xmlPath2 ) )
                     c.IncludeXmlComments( xmlPath2 );

                 //
                 c.DescribeAllEnumsAsStrings();

                 //
                 //c.AddFluentValidationRules();

                 //http://wmpratt.com/part-ii-swagger-and-asp-net-web-api-enabling-oauth2/
                 c.OperationFilter<AuthorizeCheckOperationFilter>();

                 //c.AddSecurityDefinition( "oauth2", new OAuth2Scheme {
                 //    Type = "oauth2",
                 //    Flow = "implicit",
                 //    AuthorizationUrl = $"{Configuration.GetValue<string>( "IdentityServer" )}/connect/authorize",
                 //    TokenUrl = $"{Configuration.GetValue<string>( "IdentityServer" )}/connect/token",
                 //    Scopes = new Dictionary<string, string>()
                 //    {
                 //        { "api", "Syinpo API" }
                 //    }
                 //} );
             } );

            // 请求限制
            services.Configure<SysOptions>( Configuration.GetSection( "Sys" ) );
            services.Configure<CacheOptions>( Configuration.GetSection( "Cache" ) );
            services.AddMemoryCache();

            // cache
            var cacheOptions = Configuration.GetSection("Cache").Get<CacheOptions>();
            services.AddStackExchangeRedisCache( options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
                options.InstanceName = cacheOptions.RedisInstanceName;
            } );
            services.AddDistributedMemoryCache();


            //hangfire
            services.Configure<HangfireOptions>(Configuration.GetSection("Hangfire"));
            var hangfireOptions = Configuration.GetSection("Hangfire").Get<HangfireOptions>();


            // task
            if (hangfireOptions.UseHangfire ) {
                string taskConnectionString = Configuration.GetConnectionString( "HangfireConnection" );
                services.AddHangfire( x => x.UseSqlServerStorage( taskConnectionString ) );
                services.AddHangfireServer();
                JobStorage.Current = new SqlServerStorage( taskConnectionString );
                HangfireRegister.Register();
            }

            // IoC & DI
            services.AddAutofac();
            var iocProvider = IoCRegister.Register( services, Configuration );
            IoC.Init( iocProvider.Item1, iocProvider.Item2 );

            // task
            if(hangfireOptions.UseHangfire ) {
                GlobalConfiguration.Configuration.UseAutofacActivator( iocProvider.Item2, false );
            }

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

                 endpoints.MapHub<DeviceHub>( "/hubs/device" );

                 endpoints.MapControllerRoute( "default", "{controller=Home}/{action=Index}/{id?}" );
                 endpoints.MapDefaultControllerRoute();
             } );

            app.UseSwagger( c => c.SerializeAsV2 = true );
            var hangfireOptions = Configuration.GetSection("Hangfire").Get<HangfireOptions>();
            if (hangfireOptions.UseHangfire ) {
                app.UseHangfireDashboard();
            }


            // 版权
            if( 1 == 2 ) {
                Thread thread = new Thread( new ThreadStart( SelfVerification ) );
                thread.IsBackground = true;
                thread.SetApartmentState( ApartmentState.STA );
                thread.Start();
            }
        }

        private static void SelfVerification() {
            Thread.Sleep( TimeSpan.FromSeconds( (double)new Random().Next( 10, 25 ) ) );

            bool result = false;
            var puk = CommonHelper.MapPath( "~/App_Data/publicKey.xml" );
            string publicKey = "";
            if( File.Exists( puk ) ) {
                publicKey = File.ReadAllText( puk );

                var licenseValidator = new LicenseValidator( publicKey, CommonHelper.MapPath( "~/App_Data/license.lic" ) );
                try {
                    licenseValidator.AssertValidLicense();

                    //MessageBox.Show( $"License is valid to {licenseValidator.Name}, date: {licenseValidator.ExpirationDate}" );

                    result = true;
                }
                catch( Exception ex ) {
                    Log.Error( "sn:" + ex );
                    throw ex;
                }
            }

            if( !result ) {
                Log.Error( "sn" );
                throw new InvalidOleVariantTypeException( "sn" );
            }
        }
    }
}

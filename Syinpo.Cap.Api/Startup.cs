using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using FluentValidation.Attributes;
using Hangfire;
using Hangfire.SqlServer;
using IdentityServer4.AccessTokenValidation;
using Syinpo.BusinessLogic.SignalR.Hubs;
using Syinpo.Core;
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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Ben.Diagnostics;
using DotNetCore.CAP.Messages;
using HealthChecks.UI.Client;
using Syinpo.BusinessLogic.Caches;
using Syinpo.BusinessLogic.Content;
using Syinpo.BusinessLogic.Monitor;
using Syinpo.BusinessLogic.Notifications.OfflineHandlers;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core.Caches;
using Syinpo.Core.Cap;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.EasyLicense.License.Validator;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.TracerModule;
using Syinpo.Core.SignalBus;
using Syinpo.Unity.AspNetCore.HealthCheck;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Export;
using OpenTelemetry.Trace.Samplers;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using Microsoft.AspNetCore.Identity;
using Syinpo.Unity.Redis;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;

namespace Syinpo.Cap.Api {
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


            services.AddHttpClient();
            services.AddHttpClient( "monitor" );
            services.AddHealthChecks().AddCheck<RandomHealthCheck>( "random" );


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
            var levelSwitch = new LoggingLevelSwitch( Serilog.Events.LogEventLevel.Warning );
            if( string.IsNullOrEmpty( seqServer ) ) {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy( levelSwitch )
                    .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "DotNetCore.CAP", LogEventLevel.Error )
                    .MinimumLevel.Override( "Microsoft.Extensions.Http", LogEventLevel.Warning )
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile( pathFormat: Path.Combine( AppContext.BaseDirectory, "logs\\log-{Date}.log" ) )
                    .CreateLogger();
            }
            else {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy( levelSwitch )
                    .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "DotNetCore.CAP", LogEventLevel.Error )
                    .MinimumLevel.Override( "Microsoft.Extensions.Http", LogEventLevel.Warning )
                    .Enrich.FromLogContext()
                    .WriteTo.Seq( seqServer )
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


            // 请求限制
            services.Configure<SysOptions>( Configuration.GetSection( "Sys" ) );
            services.Configure<CacheOptions>( Configuration.GetSection( "Cache" ) );
            services.AddHostedService<HealthCheckWorker>();


            // cache
            var cacheOptions = Configuration.GetSection( "Cache" ).Get<CacheOptions>();
            services.AddStackExchangeRedisCache( options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
                options.InstanceName = cacheOptions.RedisInstanceName;
                options.ConfigurationOptions = ConnectionOptions.Option;
            } );
            services.AddDistributedMemoryCache();


            //signalr
            services.Configure<SqlBusOptions>( Configuration.GetSection( "SqlBus" ) );
            var sqlBusOptions = Configuration.GetSection( "SqlBus" ).Get<SqlBusOptions>();
            if( sqlBusOptions.UseSignalrRedis ) {
                services.AddSignalR( p =>
                {
                    p.EnableDetailedErrors = true;
                    p.ClientTimeoutInterval = TimeSpan.FromSeconds( 20 );
                    p.HandshakeTimeout = TimeSpan.FromSeconds( 15 );
                    p.KeepAliveInterval = TimeSpan.FromSeconds( 5 );
                } ).AddStackExchangeRedis( cacheOptions.RedisConfiguration, options =>
                {
                    options.Configuration.ChannelPrefix = "SigRis";
                } );
            }
            else {
                services.AddSignalR( p =>
                {
                    p.EnableDetailedErrors = true;
                    p.ClientTimeoutInterval = TimeSpan.FromSeconds( 20 );
                    p.HandshakeTimeout = TimeSpan.FromSeconds( 15 );
                    p.KeepAliveInterval = TimeSpan.FromSeconds( 5 );
                } );
            }

            //signalrbus
            var sysOptions = Configuration.GetSection( "Sys" ).Get<SysOptions>();
            if( sysOptions.SignalrBus.UserSignalrBus ) {
                services.AddSignalrBus();
            }

            //hangfire
            services.Configure<HangfireOptions>( Configuration.GetSection( "Hangfire" ) );
            var hangfireOptions = Configuration.GetSection( "Hangfire" ).Get<HangfireOptions>();
            if( hangfireOptions.UseHangfire ) {
                string taskConnectionString = Configuration.GetConnectionString( "HangfireConnection" );
                services.AddHangfire( x => x.UseSqlServerStorage( taskConnectionString ) );

                if( hangfireOptions.UseHangfireServer ) {
                    services.AddHangfireServer();
                }

                JobStorage.Current = new SqlServerStorage( taskConnectionString );

                if( hangfireOptions.UseHangfireServer ) {
                    HangfireRegister.Register();
                }
            }


            // Monitor
            services.Configure<MonitorOptions>( Configuration.GetSection( "Monitor" ) );
            var monitorOptions = Configuration.GetSection( "Monitor" ).Get<MonitorOptions>();
            services.AddSingleton<MonitorExporter>();
            services.AddOpenTelemetry( ( sp, builder ) =>
            {
                builder.SetSampler( new AlwaysSampleSampler() );
                builder.UseMonitor( sp ).AddRequestCollector().AddDependencyCollector();
            } );
            services.AddScoped( resolver => resolver.GetService<TracerFactory>().GetTracer( "syinpo-api-tracer" ) );
            if( monitorOptions.UseMonitor ) {
                services.AddHostedService<MonitorWorker>();
            }


            // 服务总线
            services.Configure<CapBusOptions>( Configuration.GetSection( "CapBus" ) );
            var capBusOptions = Configuration.GetSection( "CapBus" ).Get<CapBusOptions>();
            services.AddCap( x =>
            {
                string capConnectionString = Configuration.GetConnectionString( "CapConnection" );
                x.UseSqlServer( capConnectionString );

                string rabbitmqConnectionString = capBusOptions.RabbitMQConnection;
                x.UseRabbitMQ( mq =>
                {
                    mq.UserName = "admin";
                    mq.Password = "admin";
                    mq.VirtualHost = "/";
                    mq.Port = 5672;
                    mq.HostName = rabbitmqConnectionString;
                } );

                x.FailedRetryCount = 2;
                x.FailedRetryInterval = 60 * 5;


                x.FailedThresholdCallback = ( m ) =>
                {
                    Log.Error( $@"事件总线处理失败：A message of type {m.MessageType} failed after executing {x.FailedRetryCount} several times, requiring manual troubleshooting. Message name: {m.Message.GetName()}, id: {m.Message.GetId()}, value: {m.Message.Value.ToJson()}" );
                };

                x.UseDashboard();

                x.Version = capBusOptions.VersionName;

                x.ConsumerThreadCount = 8;
            } );

            // IoC & DI
            services.AddAutofac();
            var iocProvider = IoCRegister.Register( services, Configuration );
            IoC.Init( iocProvider.Item1, iocProvider.Item2 );

            // task
            if( hangfireOptions.UseHangfire ) {
                GlobalConfiguration.Configuration.UseAutofacActivator( iocProvider.Item2, false );
            }

            return iocProvider.Item1;
        }

        public void Configure( IApplicationBuilder app, IHostingEnvironment env  ) {
            if( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            //app.UseBlockingDetection();

            //errors
            app.UseException();
            app.UseRouting();

            // Monitor
            var monitorOptions = Configuration.GetSection( "Monitor" ).Get<MonitorOptions>();
            if( monitorOptions.UseMonitor ) {
                app.UseHttpLog();
            }

            app.UseCors( "AllowAllPolicy" );
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticFiles( new StaticFileOptions() {
                FileProvider = new PhysicalFileProvider( Path.Combine( Directory.GetCurrentDirectory(), @"App_Data" ) ),
                RequestPath = new PathString( "/StaticFiles" )
            } );


            var useHttps = Configuration.GetValue<bool?>( "UseHttps" );
            if( useHttps.HasValue && useHttps.Value ) {
                app.UseHttpsRedirection();
            }

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks( "/health", new HealthCheckOptions() {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                } );

                endpoints.MapHub<DeviceHub>( "/hubs/device" );

                endpoints.MapControllerRoute( "default", "{controller=Home}/{action=Index}/{id?}" );
                endpoints.MapDefaultControllerRoute();
            } );

            var hangfireOptions = Configuration.GetSection( "Hangfire" ).Get<HangfireOptions>();
            if( hangfireOptions.UseHangfire && hangfireOptions.UseHangfireServer ) {
                app.UseHangfireDashboard();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using EFCoreSecondLevelCacheInterceptor;
using Syinpo.BusinessLogic.Caches;
using Syinpo.BusinessLogic.Content;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Jobs;
using Syinpo.BusinessLogic.Monitor;
using Syinpo.BusinessLogic.Notifications.SendHandlers;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Container;
using Syinpo.Core.Data;
using Syinpo.Core.Domain.MonitorPoco;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Hangfire;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Monitor.TracerModule;
using Syinpo.Core.Reflection;
using Syinpo.Core.SignalBus.Client;
using Syinpo.Model;
using Syinpo.Model.Dto.Users;
using Syinpo.Unity.AspNetCore.Routes;
using Syinpo.Unity.Bogus;
using Syinpo.Unity.EfCore;
using Syinpo.Unity.Redis;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.ObjectPool;
using PipelineNet.Middleware;
using PipelineNet.MiddlewareResolver;
using Z.EntityFramework.Extensions;

namespace Syinpo.Unity.Autofac {
    public class IoCRegister {

        public static Tuple<IServiceProvider, IContainer> Register( IServiceCollection services, IConfiguration configuration ) {
            var builder = new ContainerBuilder();

            // asp.net core
            var sqlBusOptions = configuration?.GetSection( "SqlBus" ).Get<SqlBusOptions>() ?? new SqlBusOptions { };
            if( configuration != null ) {
                services.AddSingleton<IRouteAnalyzer, RouteAnalyzer>();


                // EfSecondLevelCache
                if( sqlBusOptions.UseEfSecondLevelCache ) {
                    services.AddEFSecondLevelCache( options => options.UseMemoryCacheProvider( CacheExpirationMode.Absolute, TimeSpan.FromMinutes( 5 ) ).DisableLogging( true ) );
                }

                // DbContext
                services.AddDbContext<SyinpoContext>( ( serviceProvider, options ) =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseSqlServer( configuration.GetConnectionString( "DefaultConnection" ),
                        sqlServerOptionsBuilder =>
                        {
                            sqlServerOptionsBuilder
                                .CommandTimeout( (int)TimeSpan.FromMinutes( 1 ).TotalSeconds )
                                //.EnableRetryOnFailure()
                                .MigrationsAssembly( typeof( IoC ).Assembly.FullName );
                        } );
                    //  options.AddInterceptors( new HintCommandInterceptor() );
                    options.ReplaceService<IQuerySqlGeneratorFactory, CustomSqlServerQuerySqlGeneratorFactory>();

                    if( sqlBusOptions.UseEfSecondLevelCache ) {
                        options.AddInterceptors( serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>() );
                    }

                }  );
                services.AddDbContext<SyinpoReadOnlyContext>( ( serviceProvider, options ) =>
                 {
                     options.UseLazyLoadingProxies();
                     options.UseSqlServer( configuration.GetConnectionString( "DefaultReadOnlyConnection" ),
                         sqlServerOptionsBuilder =>
                         {
                             sqlServerOptionsBuilder
                                 .CommandTimeout( (int)TimeSpan.FromMinutes( 1 ).TotalSeconds )
                                 //.EnableRetryOnFailure()
                                 .MigrationsAssembly( typeof( IoC ).Assembly.FullName );
                         } );
                     // options.AddInterceptors( new HintCommandInterceptor() );
                     options.ReplaceService<IQuerySqlGeneratorFactory, CustomSqlServerQuerySqlGeneratorFactory>();

                     if( sqlBusOptions.UseEfSecondLevelCache ) {
                         options.AddInterceptors( serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>() );
                     }
                 } );
                services.AddDbContext<SyinpoMonitorContext>( options =>
                {
                    options.UseLazyLoadingProxies();
                    options.UseSqlServer( configuration.GetConnectionString( "MonitorConnection" ) );
                } );


                // IDbContext
                var defaultContextFactory = configuration[ "DefaultContextFactory" ] ?? "DefaultConnection";
                builder.RegisterType<SyinpoReadOnlyContext>().Named<IDbContext>( "SyinpoReadOnlyContext" ).InstancePerLifetimeScope();
                builder.Register( context =>
                {
                    var db = new SyinpoMonitorContext( context.Resolve<DbContextOptions<SyinpoMonitorContext>>() );
                    return db;
                } ).Named<IDbContext>( "monitor_dbcontext" ).InstancePerLifetimeScope();
                services.AddScoped<IDatabaseHelper, DatabaseHelper>();

                if( defaultContextFactory == "MonitorConnection" ) {
                    services.AddScoped<IDbContext, SyinpoMonitorContext>();

                    EntityFrameworkManager.ContextFactory = context =>
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<SyinpoMonitorContext>();
                        optionsBuilder.UseSqlServer( configuration.GetConnectionString( "MonitorConnection" ) );
                        return new SyinpoMonitorContext( optionsBuilder.Options );
                    };
                }
                else {
                    services.AddScoped<IDbContext, SyinpoContext>();
                   // builder.RegisterType<SyinpoContext>().As<IDbContext>().InstancePerLifetimeScope();

                    EntityFrameworkManager.ContextFactory = context =>
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<SyinpoContext>();
                        optionsBuilder.UseSqlServer( configuration.GetConnectionString( "DefaultConnection" ) );
                        return new SyinpoContext( optionsBuilder.Options );
                    };
                }


                // DbContextFactory
                builder.RegisterType<DbContextFactory>()
                    .WithParameter(
                        new ResolvedParameter(
                            ( pi, ctx ) => pi.ParameterType == typeof( IDbContext ) && pi.Name == "readDbContext",
                            ( pi, ctx ) => defaultContextFactory == "MonitorConnection"
                                ? ctx.ResolveNamed<IDbContext>( "monitor_dbcontext" )
                                : ( sqlBusOptions.UseReadWriteSpit
                                    ? ctx.ResolveNamed<IDbContext>( "SyinpoReadOnlyContext" )
                                    : ctx.Resolve<IDbContext>() ) ) )
                    .WithParameter(
                        new ResolvedParameter(
                            ( pi, ctx ) => pi.ParameterType == typeof( IDbContext ) && pi.Name == "writeDbContext",
                            ( pi, ctx ) =>
                                defaultContextFactory == "MonitorConnection" ?
                                    ctx.ResolveNamed<IDbContext>( "monitor_dbcontext" ) :
                                    ctx.Resolve<IDbContext>() ) )
                    .WithParameter( "settings", sqlBusOptions )
                    .As<DbContextFactory>().InstancePerLifetimeScope();



                {

                    builder.RegisterType<GenericRepository<RequestLog>>().As<IGenericRepository<RequestLog>>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<GenericRepository<ResponseSnap>>().As<IGenericRepository<ResponseSnap>>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<GenericRepository<TrafficStatist>>().As<IGenericRepository<TrafficStatist>>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<GenericRepository<SqlSnap>>().As<IGenericRepository<SqlSnap>>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<GenericRepository<ExceptionSnap>>().As<IGenericRepository<ExceptionSnap>>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<GenericRepository<TrackSnap>>().As<IGenericRepository<TrackSnap>>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();


                    builder.RegisterType<RequestLogService>().As<IRequestLogService>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<ResponseLogService>().As<IResponseLogService>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                    builder.RegisterType<TrafficStatistService>().As<ITrafficStatistService>()
                        .WithParameter( ResolvedParameter.ForNamed<IDbContext>( "monitor_dbcontext" ) )
                        .InstancePerLifetimeScope();
                }


                services.AddEntityFrameworkSqlServer();
                services.AddEntityFrameworkProxies();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddTransient<IActionContextAccessor, ActionContextAccessor>();


                services.AddScoped( typeof( IGenericRepository<> ), typeof( GenericRepository<> ) );


                // storage
                var storageCloud = configuration[ "StorageCloud" ] ?? "oss";
                if( storageCloud == "obs" ) {
                    //services.AddScoped<IStorageService, ObsStorageService>();
                }
                else {
                    //services.AddScoped<IStorageService, OssStorageService>();
                }

                // AOP


                // 数据容器
                builder.RegisterType<RedisContainerProvider>().As<IDataContainer>().InstancePerLifetimeScope();
            }


            // service
            builder.RegisterType<Current>().As<ICurrent>().InstancePerLifetimeScope();
            builder.RegisterType<FakerDto>().As<IFaker>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();
            builder.RegisterType<EventQueueService>().As<IEventQueueService>().InstancePerLifetimeScope();

            // SignalR
            builder.RegisterType<SignalRRealTimeNotifier>().As<IRealTimeNotifier>().InstancePerLifetimeScope();
            builder.RegisterType<PushService>().As<IPushService>().InstancePerLifetimeScope();
            //builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            //builder.RegisterType<DeviceOfflineNotificationJob>();
            builder.RegisterAssemblyTypes( typeof( IBackgroundJob<> ).GetTypeInfo().Assembly ).AsClosedTypesOf( typeof( IBackgroundJob<> ) ).AsImplementedInterfaces();

            //online
            if ( sqlBusOptions.UseSignalrRedis ) {
                services.AddSingleton<IOnlineManager, OnlineRedisManager>();
            }
            else {
                services.AddSingleton<IOnlineManager, OnlineManager>();
            }

            //
            // builder.RegisterType<EventQueueJob>().InstancePerLifetimeScope();


            // Assembly
            builder.RegisterType<AssemblyFinder>().As<IAssemblyFinder>().SingleInstance();
            builder.RegisterType<TypeFinder>().As<ITypeFinder>().SingleInstance();
            builder.RegisterType<TypeResolve>().As<ITypeResolve>().SingleInstance();

            // cache
            var cacheOptions = configuration?.GetSection( "Cache" ).Get<CacheOptions>();
            if( cacheOptions?.UseCache == DistributedCacheNamedEnum.SqlServer.ToString() ) {
                builder.RegisterType<SqlServerCache>().As<IDistributedCache>().SingleInstance();
            }
            else if( cacheOptions?.UseCache == DistributedCacheNamedEnum.Redis.ToString() ) {
                builder.RegisterType<RedisCache>().As<IDistributedCache>().SingleInstance();
            }
            else {
                builder.RegisterType<MemoryDistributedCache>().As<IDistributedCache>().SingleInstance();
            }
            builder.RegisterType<Cache>().As<ICache>().InstancePerLifetimeScope();
            builder.RegisterType<RedisCacheWrapper>().As<IRedisCacheWrapper>().SingleInstance();
            builder.RegisterType<Md5Cache>().As<IMd5Cache>()
                .WithParameter( "maxCount", 100000 ).SingleInstance();
            builder.RegisterType<Md5Cache>().Named<IMd5Cache>( "DeviceApi" )
                .WithParameter( "maxCount", 256 ).SingleInstance();
            builder.RegisterType<Md5Cache>().Named<IMd5Cache>( "DeviceHub" )
                .WithParameter( "maxCount", 100000  ).SingleInstance();
            builder.RegisterType<CacheEvent>().As<ICacheEvent>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes( typeof( ICacheSubscribe<> ).GetTypeInfo().Assembly ).AsClosedTypesOf( typeof( ICacheSubscribe<> ) ).AsImplementedInterfaces();

            // Monitor
            var monitorOptions = configuration?.GetSection( "Monitor" ).Get<MonitorOptions>();
            builder.RegisterType<PackStore<HttpLog>>().As<IPackStore<HttpLog>>()
                 .WithParameter( "packStoreOptions", new PackStoreOptions {
                     TempStorePath = monitorOptions?.RequestStorePath,
                     GatewayApiRoute = "api/batch/logs"
                 } )
                .SingleInstance();
            builder.RegisterType<PackStore<MonitorEvent>>().As<IPackStore<MonitorEvent>>()
                .WithParameter( "packStoreOptions", new PackStoreOptions {
                    TempStorePath = monitorOptions?.ResponseStorePath,
                    GatewayApiRoute = "api/batch/events"
                } )
                .SingleInstance();
            builder.RegisterType<PackStore<TimeData>>().As<IPackStore<TimeData>>()
                .WithParameter( "packStoreOptions", new PackStoreOptions {
                    TempStorePath = monitorOptions?.TimeDataStorePath,
                    GatewayApiRoute = "api/batch/timedata"
                } )
                .SingleInstance();

            // signalrbus
            services.AddSingleton<SignalrClient>();

            // task
            builder.RegisterType<HangfireBackgroundJobManager>().As<IBackgroundJobManager>().InstancePerLifetimeScope();

            // PipelineNet
            builder.RegisterType<PipelineNetMiddlewareResolver>().As<IMiddlewareResolver>().SingleInstance();

            // mediatR
            var assembly = typeof( UserService ).Assembly;
            builder.AddMediatR( assembly );

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<>),
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>),
            };

            foreach( var mediatrOpenType in mediatrOpenTypes ) {
                builder
                    .RegisterAssemblyTypes( typeof( UserDto ).GetTypeInfo().Assembly )
                    .AsClosedTypesOf( mediatrOpenType )
                    .AsImplementedInterfaces();
            }

            // build
            builder.Populate( services );
            var appContainer = builder.Build();
            return Tuple.Create<IServiceProvider, IContainer>( new AutofacServiceProvider( appContainer ), appContainer );
        }
    }
}

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using AutoMapper;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Reflection;
using Syinpo.Unity.Autofac;
using Syinpo.Unity.AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Syinpo.Tests.Core {
    public class BaseTest {
        [SetUp]
        public virtual void Start() {
            var serviceProvider = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath( Directory.GetCurrentDirectory() )
                .AddJsonFile( "appsettings.json" );
            var config = builder.Build();
            serviceProvider.AddSingleton<IConfiguration>( context => config );

            var env = new SysHostEnvironment {
                EnvironmentName = Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" ),
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                ContentRootFileProvider = new PhysicalFileProvider( AppDomain.CurrentDomain.BaseDirectory ),
                WebRootPath = AppDomain.CurrentDomain.BaseDirectory,
                WebRootFileProvider = new PhysicalFileProvider( AppDomain.CurrentDomain.BaseDirectory ),
            };
            serviceProvider.AddSingleton( typeof( IWebHostEnvironment ), env );

            var seqServer = config.GetValue<string>( "SeqServer" );
            var levelSwitch = new LoggingLevelSwitch( Serilog.Events.LogEventLevel.Information );
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy( levelSwitch )
                .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                .Enrich.FromLogContext()
                .WriteTo.Seq( seqServer )
                .WriteTo.Stackify( restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information )
                .CreateLogger();
            serviceProvider.AddLogging( loggingBuilder => loggingBuilder.AddSerilog( dispose: true ) );

            // asp.net
            serviceProvider.AddSingleton(PlatformServices.Default.Application);
            serviceProvider.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceProvider.AddSingleton<DiagnosticSource>( new DiagnosticListener( "Microsoft.AspNetCore" ) );
            serviceProvider.AddSingleton<DiagnosticListener>( new DiagnosticListener( "Microsoft.AspNetCore" ) );
            serviceProvider.Configure<RazorViewEngineOptions>( options =>
            {

            } );
            serviceProvider.AddRazorPages();
            serviceProvider.AddMvc();


            // cache
            serviceProvider.Configure<CacheOptions>( config.GetSection( "Cache" ) );
            var cacheOptions = config.GetSection( "Cache" ).Get<CacheOptions>();
            serviceProvider.AddStackExchangeRedisCache( options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
                options.InstanceName = cacheOptions.RedisInstanceName;
            } );
            serviceProvider.AddDistributedMemoryCache();


            //automapper
            serviceProvider.AddAutoMapper( typeof( BaseTest ) );
            MapperRegister.Register();


            var result = IoCRegister.Register( serviceProvider, config );
            Syinpo.Core.IoC.Init( result.Item1, result.Item2 );
        }

        [TearDown]
        public virtual void End() {

        }
    }
}



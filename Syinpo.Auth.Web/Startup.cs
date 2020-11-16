using Autofac.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Syinpo.Core;
using Syinpo.Core.Helpers;
using Syinpo.Unity.AspNetCore.Filters;
using Syinpo.Unity.Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IdentityModel.Client;
using Syinpo.Unity.AutoMapper;
using Microsoft.Extensions.Options;
using Serilog.Events;

namespace Syinpo.Auth.Server {

    public class Startup {

        public Startup( IConfiguration configuration ) {
            Configuration = configuration;
        }

        public IConfiguration Configuration {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices( IServiceCollection services ) {
            // 跨域
            services.AddCors( o => o.AddPolicy( "AllowAllPolicy", builder =>
            {
                builder
                    .SetIsOriginAllowed( origin => true )
                    .WithMethods( "GET", "POST", "DELETE", "OPTIONS", "PUT" )
                    .AllowAnyHeader()
                    .AllowCredentials();
            } ) );

            services.AddControllers( config =>
            {
                config.Filters.Add( typeof( CustomExceptionFilter ) );
            } ).AddControllersAsServices();

            // tokenClient
            services.AddHttpClient();
            services.AddHttpClient( "token_client",
                client => client.BaseAddress = new Uri( $"{CommonHelper.GetWebLocation()}connect/token" ) );

            // log
            var seqServer = Configuration.GetValue<string>( "SeqServer" );
            var levelSwitch = new LoggingLevelSwitch( Serilog.Events.LogEventLevel.Debug );
            if( string.IsNullOrEmpty( seqServer ) ) {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy( levelSwitch )
                    .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile( pathFormat: Path.Combine( AppContext.BaseDirectory, "logs\\log-{Date}.log" ) )
                    .CreateLogger();
            }
            else {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy( levelSwitch )
                    .MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
                    .MinimumLevel.Override( "Microsoft.EntityFrameworkCore", LogEventLevel.Warning )
                    .Enrich.FromLogContext()
                    .WriteTo.Seq( seqServer )
                    .CreateLogger();
            }

            services.AddLogging( loggingBuilder => loggingBuilder.AddSerilog( dispose: true ) );

            //automapper
            services.AddAutoMapper( typeof( Startup ) );
            MapperRegister.Register();

            // 授权服务器
            // http://192.168.50.58:6060/.well-known/openid-configuration
            AddIdentityServer( services );

            //services.AddAuthentication();
            //services.AddAuthentication( "Bearer" )
            //    .AddIdentityServerAuthentication( options =>
            //    {
            //        options.Authority = "https://localhost:44372";
            //        options.RequireHttpsMetadata = false;

            //        options.ApiName = "api";
            //    } );

            // cache
            services.AddDistributedMemoryCache();

            // 请求限制
            services.AddMemoryCache();

            // IoC & DI
            services.AddAutofac();

            var serviceProvider = IoCRegister.Register( services, Configuration );
            IoC.Init( serviceProvider.Item1, serviceProvider.Item2 );
            return serviceProvider.Item1;
        }

        private void AddIdentityServer( IServiceCollection services ) {
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            string authCenterConnectionString = Configuration.GetConnectionString( "AuthCenterConnection" );

            var migrationsAssembly = typeof( Startup ).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer( option =>
                {
                    option.IssuerUri = Configuration.GetValue<string>( "IdentityServer" );
                } )
                .AddSigningCredential( signingKey, IdentityServer4.IdentityServerConstants.RsaSigningAlgorithm.RS256 )
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore( options =>
                 {
                     options.ConfigureDbContext = b =>
                         b.UseSqlServer( authCenterConnectionString,
                             sql => sql.MigrationsAssembly( migrationsAssembly ) );
                 } )
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore( options =>
                 {
                     options.ConfigureDbContext = b =>
                         b.UseSqlServer( authCenterConnectionString,
                             sql => sql.MigrationsAssembly( migrationsAssembly ) );

                     // this enables automatic token cleanup. this is optional.
                     options.EnableTokenCleanup = true;
                 } )
                .AddProfileService<IdentityServerProfileService>()
                .AddResourceOwnerValidator<OAuthResourceOwnerPasswordValidator>();

            // Credential
            //.AddDeveloperSigningCredential()
            //.AddTestUsers( Config.GetUsers() )

            // static
            //.AddInMemoryApiResources( Config.GetApis() )
            //.AddInMemoryIdentityResources( Config.GetIdentityResources() )
            //.AddInMemoryClients( Config.GetClients() )
            //.AddTestUsers( TestUsers.Users )
            //.AddDeveloperSigningCredential( persistKey: false );
        }

        private void ApplyIdentityServerMigrations( IApplicationBuilder app ) {
            using( var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope() ) {
                // the database.Migrate command will apply all pending migrations and will create the database if it is not created already.
                var persistedGrantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantContext.Database.Migrate();

                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationContext.Database.Migrate();
            }
        }

        private void SeedData( IApplicationBuilder app ) {
            using( var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope() ) {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if( !configurationContext.Clients.Any() ) {
                    foreach( var client in Config.GetClients() ) {
                        configurationContext.Clients.Add( client.ToEntity() );
                    }
                    configurationContext.SaveChanges();
                }

                if( !configurationContext.IdentityResources.Any() ) {
                    foreach( var resource in Config.GetIdentityResources() ) {
                        configurationContext.IdentityResources.Add( resource.ToEntity() );
                    }
                    configurationContext.SaveChanges();
                }

                if( !configurationContext.ApiResources.Any() ) {
                    foreach( var resource in Config.GetApis() ) {
                        configurationContext.ApiResources.Add( resource.ToEntity() );
                    }
                    configurationContext.SaveChanges();
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env ) {
            if( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors( "AllowAllPolicy" );
            app.UseHttpsRedirection();
            app.UseEndpoints( endpoints =>
            {
                endpoints.MapControllers();
            } );

            // ApplyIdentityServerMigrations( app );
            // SeedData( app );

            app.UseIdentityServer();

            // app.UseAuthentication();
        }
    }
}
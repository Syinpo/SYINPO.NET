using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CachingFramework.Redis;
using CachingFramework.Redis.Serializers;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Syinpo.Unity.Redis {

    public class RedisCacheWrapper : IDisposable, IRedisCacheWrapper {
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim( 1, 1 );
        private volatile ConnectionMultiplexer _connection;
        private readonly RedisCacheOptions _options;
        private readonly string _instance;
        private readonly string _config;
        private IDatabase _dataBase;
        private IServer _server;

        public RedisCacheWrapper( IOptions<RedisCacheOptions> optionsAccessor, IOptions<CacheOptions> cacheOptions ) {
            if( optionsAccessor == null )
                throw new ArgumentNullException( nameof( optionsAccessor ) );
            this._options = optionsAccessor.Value;
            this._instance = this._options.InstanceName ?? string.Empty;

            if( cacheOptions.Value.UseCache == DistributedCacheNamedEnum.Redis.ToString() ) {
                CachingFramework.Redis.RedisContext.DefaultSerializer = new JsonSerializer();
                _config = _options.Configuration;
            }
        }

        public async Task<IConnectionMultiplexer> GetConnect() {
            return await Task.FromResult( ConnectionOptions.Connection );
            //this._connectionLock.Wait();
            //try {

            //    if( this._connection != null && _connection.IsConnected )
            //        return _connection;

            //    this._connection = string.IsNullOrEmpty( _config ) ?
            //       await ConnectionMultiplexer.ConnectAsync( "localhost:6379" ) :
            //       await ConnectionMultiplexer.ConnectAsync( _config );

            //    return _connection;
            //}
            //finally {
            //    this._connectionLock.Release();
            //}
        }

        public async Task<IServer> GetServer() {
            if( _server != null )
                return _server;

            _server = ( await GetConnect() ).GetServer( ( await GetConnect() ).GetEndPoints()[ 0 ] );
            return _server;
        }

        public async Task<IDatabase> GetDatabase() {
            if( _dataBase != null )
                return _dataBase;

            _dataBase = ( await GetConnect() ).GetDatabase( -1, (object)null );
            return _dataBase;
        }

        public DateTime ServerNow {
            get {
                var serverNow = GetServer().Result.Time();
                return serverNow;
            }
        }

        public async Task<RedLockFactory> GetRedisLockFactory() {
            var con = await GetConnect() as ConnectionMultiplexer;

            var multiplexers = new List<RedLockMultiplexer>
            {
                con,
            };

            return RedLockFactory.Create( multiplexers );
        }


        public async Task<RedisContext> RedisContext() {
            var contection = await GetConnect();
            return new RedisContext( contection );
        }


        public void Dispose() {
            if( this._connection != null )
                this._connection.Close( true );
        }
    }

    public static class ConnectionOptions {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>( () =>
        {
            return ConnectionMultiplexer.Connect( Option );
        } );

        public static ConnectionMultiplexer Connection {
            get {
                return lazyConnection.Value;
            }
        }

        public static ConfigurationOptions Option {
            get {
                var options = IoC.Resolve<IOptions<CacheOptions>>().Value;
                ConfigurationOptions config = new ConfigurationOptions();
                config.EndPoints.Add( options.RedisConfiguration );
                config.AbortOnConnectFail = false;
                config.ConnectRetry = 5;
                config.ConnectTimeout = 1000;
                return config;
            }
        }
    }
}

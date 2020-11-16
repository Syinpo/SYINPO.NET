using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CachingFramework.Redis;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace Syinpo.Unity.Redis {
    public interface IRedisCacheWrapper {
        Task<IConnectionMultiplexer> GetConnect();

        Task<IServer> GetServer();

        Task<IDatabase> GetDatabase();

        DateTime ServerNow {get;}

        Task<RedisContext> RedisContext();

        Task<RedLockFactory> GetRedisLockFactory();

        void Dispose();
    }
}
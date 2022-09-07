using Common.Extensions;
using Domain;
using Interfaces;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class RedisCacheService : IRedisCache
    {
        private readonly IConnectionMultiplexer _multiplexer;

        public RedisCacheService(IConnectionMultiplexer multiplexer)
        {
            _multiplexer = multiplexer;
        }
        public async Task<T> TryOperation<T>(Func<IDatabase, Task<T>> op)
        {
            var db = _multiplexer.GetDatabase();
            return await op(db);
        }

        public async Task TryOperation(Func<IDatabase, Task> op)
        {
            var db = _multiplexer.GetDatabase();
            await op(db);
        }
        public async Task<T> GetEntryAsync<T>(string key)
        {
            try
            {
                return await TryOperation(async db =>
                {
                    return await db.GetEntryAsync<T>(key);
                });
            }
            catch
            {
                return default;
            }
        }
        public async Task SetEntryAsync<TValue>(string key, TValue value, TimeSpan cacheExpiry, When setWhen = When.NotExists)
        {
            await TryOperation(async db =>
                    {
                        await db.SetEntryAsync(key, value, cacheExpiry, setWhen);
                    });
        }
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> valueToCache, TimeSpan cacheExpiry) where T : class
        {
            return await TryOperation(async db =>
            {
                return await db.GetOrSetAsync<T>(key, valueToCache, cacheExpiry);
            });
        }
        public async Task<TimeSpan> Ping()
        {
            return await TryOperation(db => db.PingAsync());
        }

    }

}

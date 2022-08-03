using Interfaces;
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
    }

}

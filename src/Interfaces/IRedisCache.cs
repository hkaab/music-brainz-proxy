using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRedisCache
    {
        Task<T> TryOperation<T>(Func<IDatabase, Task<T>> op);

        Task TryOperation(Func<IDatabase, Task> op);
        Task<T> GetEntryAsync<T>(string key);
        Task SetEntryAsync<TValue>(string key, TValue value, TimeSpan cacheExpiry, When setWhen = When.NotExists);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> valueToCache, TimeSpan cacheExpiry) where T : class;
        Task<TimeSpan> Ping();
    }
}

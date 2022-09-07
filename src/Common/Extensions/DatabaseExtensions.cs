using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;
using System;

namespace Common.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task<T> GetEntryAsync<T>(this IDatabase database, string key)
        {
            var jsonValue = (string)await database.StringGetAsync(key);

            if (string.IsNullOrEmpty(jsonValue))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(jsonValue);
        }

        public static async Task SetEntryAsync<TValue>(this IDatabase db, string key, TValue value, TimeSpan cacheExpiry, When setWhen = When.NotExists)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, jsonValue, cacheExpiry, setWhen);
        }

        public static async Task<bool> RemoveEntryAsync(this IDatabase db, string key)
        {
            return await db.KeyDeleteAsync(key);
        }
        public static async Task<T> GetOrSetAsync<T>(this IDatabase db,string key,Func<Task<T>> valueToCache,TimeSpan cacheExpiry) where T : class
        {
            var cached = await db.GetEntryAsync<T>(key);
            if (cached != default(T))
            {
                return cached;
            }

            var value = await valueToCache();
            await db.SetEntryAsync(key, value, cacheExpiry, When.NotExists);
            return value;
        }
    }
}

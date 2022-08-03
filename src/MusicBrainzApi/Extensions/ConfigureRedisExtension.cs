using Common;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using StackExchange.Redis;
using System.Security.Authentication;

namespace MusicBrainzApi
{
    public static class ConfigureRedisExtension
    {
        public static void AddRedisCache(this IServiceCollection services, string connection)
        {
            var options = GetRedisConfiguration(connection);
            services.AddSingleton<IConnectionMultiplexer>(s =>
            {
                var logger = s.GetService<ILogger<ConnectionMultiplexer>>();
                Redis.Logger = logger;
                Redis.ConfigurationOptions = options;
                return Redis.Connection;
            });
            services.AddTransient<IRedisCache, RedisCacheService>();
        }

        private static ConfigurationOptions GetRedisConfiguration(string connection)
        {
            var options = ConfigurationOptions.Parse(connection);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 15000; // ms
            options.SyncTimeout = 7000;
            options.Ssl = true;
            options.SslProtocols = SslProtocols.Tls12;
            return options;
        }
    }
}

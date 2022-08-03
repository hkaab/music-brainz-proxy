using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Common
{
    public static class Redis
    {
        private static readonly Lazy<ConnectionMultiplexer> Multiplexer = CreateMultiplexer();

        public static ConfigurationOptions ConfigurationOptions { get; set; }

        public static ILogger<ConnectionMultiplexer> Logger { get; set; }

        public static ConnectionMultiplexer Connection => Multiplexer.Value;

        private static Lazy<ConnectionMultiplexer> CreateMultiplexer()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(ConfigurationOptions, Console.Out);
                connectionMultiplexer.ConnectionFailed += ConnectionMultiplexer_ConnectionFailed;
                connectionMultiplexer.InternalError += ConnectionMultiplexer_InternalError;
                connectionMultiplexer.ConnectionRestored += ConnectionMultiplexer_ConnectionRestored;
                return connectionMultiplexer;
            });
        }

        private static void ConnectionMultiplexer_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Logger.LogInformation(e.Exception, $"Connection restoring {e}");
        }

        private static void ConnectionMultiplexer_InternalError(object sender, InternalErrorEventArgs e)
        {
            Logger.LogError(e.Exception, $"Internal error in the server {e}");
        }

        private static void ConnectionMultiplexer_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Logger.LogError(e.Exception, $"Connection Multiplexer failed {e}");
        }
    }
}

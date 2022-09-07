using Microsoft.Extensions.DependencyInjection;
using MusicBrainzApi.Core;
using MusicBrainzApi.HealthChecks;

namespace MusicBrainzApi.Extensions
{
    public static class HealthCheckExtensions
    {
        public static void AddHealthChecks(this IServiceCollection services, ApplicationConfiguration appConfig)
        {
            services.AddHealthChecks()
                .AddCheck<MusicBrainzHealthCheck>("MusicBrainz Health Check")
                .AddCheck<RedisHealthCheck>("Redis Health Check");
        }
    }
}

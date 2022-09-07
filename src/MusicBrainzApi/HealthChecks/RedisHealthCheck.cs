using Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace MusicBrainzApi.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IRedisCache _cache;
        private readonly ILogger<RedisHealthCheck> _logger;

        public RedisHealthCheck(IRedisCache cache, ILogger<RedisHealthCheck> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _cache.Ping();
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Redis is unhealthy. Ping failed.", ex);
                return HealthCheckResult.Degraded(ex.Message);
            }
        }
    }

}

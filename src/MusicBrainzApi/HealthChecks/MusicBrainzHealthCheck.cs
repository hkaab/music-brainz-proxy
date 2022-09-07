using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace MusicBrainzApi.HealthChecks
{
    public class MusicBrainzHealthCheck : IHealthCheck
    {
        private readonly string _healthCheckUrl;
        private readonly HttpClient _httpClient;
        public MusicBrainzHealthCheck(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MusicBrainz");
            _healthCheckUrl = config.GetValue<string>("MusicBrainz:HealthCheckUrl");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync(_healthCheckUrl, cancellationToken);

                response.EnsureSuccessStatusCode();

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded(ex.Message);
            }
        }
    }
}
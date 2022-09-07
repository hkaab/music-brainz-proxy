using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MusicBrainzApi.Extensions
{
    public static class UseHealthCheckExtension
    {
        public static IApplicationBuilder UseHealthCheckOptions(this IApplicationBuilder app)
        {
            return app.UseHealthChecks("/healthz", GetHealthCheckOptions());
        }

        private static HealthCheckOptions GetHealthCheckOptions()
        {
            return new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(new
                    {
                        status = report.Status.ToString(),
                        errors = report.Entries.Select(e =>
                            new
                            {
                                key = e.Key,
                                value = Enum.GetName(
                                    typeof(HealthStatus),
                                    e.Value.Status)
                            })
                    });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                },
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status206PartialContent,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            };
        }
    }
}

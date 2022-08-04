using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MusicBrainzApi.Middleware
{
    public class EnforceHttpsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EnforceHttpsMiddleware> _logger;

        public EnforceHttpsMiddleware(RequestDelegate next, ILogger<EnforceHttpsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.IsHttps)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsync("Only https connections accepted");
                _logger.LogError("Http connections not allowed");
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
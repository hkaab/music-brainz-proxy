using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace MusicBrainzApi.Extensions
{
    public class RequestHeadersInitializer : ITelemetryInitializer
    {
        public const string ApplicationInsightsRequestHeaderPrefix = "Request header";
        public const string ApplicationInsightsResponseHeaderPrefix = "Response header";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHeadersInitializer(IHttpContextAccessor httpContextAccessor)
        {
            RequestHeaders = new List<string>
            {
                HeaderConstants.RequestHeaderKeys.Referer,
                HeaderConstants.RequestHeaderKeys.UserAgent,
                HeaderConstants.RequestHeaderKeys.Host,
                HeaderConstants.RequestHeaderKeys.AcceptLanguage,
                HeaderConstants.RequestHeaderKeys.Origin
            };
            ResponseHeaders = new List<string>
            {
                HeaderConstants.ResponseHeaderKeys.CacheControl,
                HeaderConstants.ResponseHeaderKeys.AllowedOrigins,
                HeaderConstants.ResponseHeaderKeys.CSP
            };
            _httpContextAccessor = httpContextAccessor;
        }

        public List<string> RequestHeaders { get; set; }

        public List<string> ResponseHeaders { get; set; }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is RequestTelemetry))
            {
                return;
            }

            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return;
            }

            var requestHeaders = context.Request
                .Headers
                .Where(x => RequestHeaders.Contains(x.Key) ||
                            x.Key.StartsWith("x-", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            requestHeaders.ForEach(header =>
            {
                var requestHeaderKeyName = $"{ApplicationInsightsRequestHeaderPrefix}: {header.Key}";
                if (!telemetry.Context.GlobalProperties.ContainsKey(requestHeaderKeyName))
                {
                    telemetry.Context.GlobalProperties.Add(requestHeaderKeyName, string.Join(Environment.NewLine, header.Value.ToEnumerable()));
                }
            });

            var responseHeaders = context.Response
                .Headers
                .Where(x => ResponseHeaders.Contains(x.Key) || x.Key.StartsWith("x-", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            responseHeaders.ForEach(header =>
            {
                var responseHeaderKeyName = $"{ApplicationInsightsResponseHeaderPrefix}: {header.Key}";
                if (!telemetry.Context.GlobalProperties.ContainsKey(responseHeaderKeyName))
                {
                    telemetry.Context.GlobalProperties.Add(responseHeaderKeyName, string.Join(Environment.NewLine, header.Value.ToEnumerable()));
                }
            });
        }
    }
}
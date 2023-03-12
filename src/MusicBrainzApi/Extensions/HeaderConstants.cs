namespace MusicBrainzApi.Extensions
{
    public static class HeaderConstants
    {
        public const string CorrelationIdHeader = "x-myobapi-requestid";

        public static class RequestHeaderKeys
        {
            public const string Referer = "Referer";
            public const string UserAgent = "User-Agent";
            public const string Host = "Host";
            public const string AcceptLanguage = "Accept-Language";
            public const string Origin = "Origin";
            public const string RequestJsonBody = "RequestJsonBody";
        }

        public static class ResponseHeaderKeys
        {
            public const string CacheControl = "Cache-Control";
            public const string AllowedOrigins = "Access-Control-Allow-Origin";
            public const string CSP = "Content-Security-Policy";
        }
    }
}
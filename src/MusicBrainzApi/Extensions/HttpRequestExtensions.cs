using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace MusicBrainzApi.Extensions
{
    public static class HttpRequestExtensions
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        private static string timeoutPropertyKey = "RequestTimeout";

        public static async Task<string> StreamToFile(this HttpRequest request, long? fileLengthLimit = 0)
        {
            var targetFilePath = Path.GetTempFileName();
            var boundary = GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                DefaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, request.Body);
            reader.BodyLengthLimit = fileLengthLimit.GetValueOrDefault() > 0 ? fileLengthLimit : null;

            using (var targetStream = File.Create(targetFilePath))
            {
                var section = await reader.ReadNextSectionAsync();

                while (section != null)
                {
                    var hasContentDispositionHeader =
                        ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out _);

                    if (hasContentDispositionHeader)
                    {
                        await section.Body.CopyToAsync(targetStream);
                    }

                    section = await reader.ReadNextSectionAsync();
                }
            }

            return targetFilePath;
        }

        public static bool IsMultiPartContentType(this HttpRequest request)
        {
            return !string.IsNullOrEmpty(request.ContentType) && request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string GetRawBodyString(this HttpContext httpContext, Encoding encoding)
        {
            var body = string.Empty;
            if (httpContext.Request.ContentLength == null || !(httpContext.Request.ContentLength > 0) ||
                !httpContext.Request.Body.CanSeek)
            {
                return body;
            }

            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(httpContext.Request.Body, encoding, true, 1024, true))
            {
                body = reader.ReadToEnd();
            }

            httpContext.Request.Body.Position = 0;
            return body;
        }

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Properties[timeoutPropertyKey] = timeout;
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Properties.TryGetValue(timeoutPropertyKey, out var value) && value is TimeSpan timeout)
            {
                return timeout;
            }

            return null;
        }

        private static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }
    }
}
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MYOB.Huxley.Utilities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MusicBrainzApi.Extensions
{
    public class AddHttpHeadersToSwagger : IOperationFilter
    {
        private const string RequestIdDescription = "Correlation ID";
        private static readonly OpenApiSchema RequestIdSchema = new OpenApiSchema { Type = "string", Example = new OpenApiString("92bd2850-601e-4daf-b820-45332a7a9fb4") };

        private readonly OpenApiHeader _requestIdHeader = new OpenApiHeader
        {
            Description = RequestIdDescription,
            Required = true,
            AllowEmptyValue = false,
            Schema = RequestIdSchema
        };

        private readonly OpenApiParameter _requestIdParameter = new OpenApiParameter
        {
            Name = HeaderConstants.CorrelationIdHeader,
            In = ParameterLocation.Header,
            Description = RequestIdDescription,
            Required = true,
            AllowEmptyValue = false,
            Schema = RequestIdSchema
        };

        private readonly OpenApiParameter _authorizationParameter = new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "IDAM JWT token",
            Required = true,
            AllowEmptyValue = false,
            Schema = new OpenApiSchema { Type = "string" }
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters?.AddRange(new[]
            {
                _authorizationParameter,
                _requestIdParameter
            });

            operation.Responses?.ForEach(r =>
            {
                r.Value.Headers.Add(new KeyValuePair<string, OpenApiHeader>(
                    HeaderConstants.CorrelationIdHeader, _requestIdHeader));
            });
        }
    }
}
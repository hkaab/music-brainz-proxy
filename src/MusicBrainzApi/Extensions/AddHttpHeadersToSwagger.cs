using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MusicBrainzApi.Extensions
{
    public class AddHttpHeadersToSwagger : IOperationFilter
    {
        // Define the _requestIdHeader field to fix CS0103 error
        private readonly OpenApiHeader _requestIdHeader = new OpenApiHeader
        {
            Description = "Request ID for tracking",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        };

        // Define the _authorizationParameter field to avoid potential errors
        private readonly OpenApiParameter _authorizationParameter = new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Access token for authorization",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        };

        // Define the _requestIdParameter field to avoid potential errors
        private readonly OpenApiParameter _requestIdParameter = new OpenApiParameter
        {
            Name = HeaderConstants.CorrelationIdHeader,
            In = ParameterLocation.Header,
            Description = "Request ID for tracking",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            // Replace AddRange with a foreach loop to avoid CS1061 error
            foreach (var parameter in new[] { _authorizationParameter, _requestIdParameter })
            {
                operation.Parameters.Add(parameter);
            }

            operation.Responses?.ToList().ForEach(r =>
            {
                r.Value.Headers.Add(new KeyValuePair<string, OpenApiHeader>(
                    HeaderConstants.CorrelationIdHeader, _requestIdHeader));
            });
        }
    }
}

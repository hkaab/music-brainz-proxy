using System;
using System.Linq;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MusicBrainzApi.Extensions
{
    public class RequestBodyInitializer : ITelemetryInitializer
    {
        public static readonly string[] PiiFields =
        {
            "Phone", "PhoneNumber", "PhoneNumbers", "Phone", "Fax", "Phone1",
            "WebAddress", "Email", "EmailAddress", "EmailAddress1", "CcToEmail", "FromEmail", "ToEmail", "ReplyToEmail", "AuthorisationEmail", "PaySlipEmail",
            "MessageBody", "DefaultSaleInvoiceEmailBody", "DefaultStatementEmailSubject", "DefaultSaleQuoteEmailSubject", "DefaultSaleQuoteEmailBody",
            "DefaultSaleInvoiceEmailSubject", "DefaultSaleInvoiceEmailBody", "Name2", "ContactName", "Name", "FromName", "ToName", "Street", "StreetAddress",
            "StreetAddress1", "StreetAddress2", "UserName", "TaxFileNumber", "UploadPassword", "Password", "ABN", "AbnBranch", "CompanyTradingName", "TradingName",
            "CompanyName", "ItemDescription", "ItemName", "Notes", "BankNumber", "BankNumberNZ", "AbaAccountNumber", "AccountNumber", "AccountNumberAU",
            "AccountNumberNZ", "AccountName", "AccountNameAU", "AccountNameNZ", "Memo", "Body", "Subject"
        };

        public static readonly string[] FileUploadRequests =
        {
            "InTrayUnlinkedDocuments", "BankTransactionImports", "EmailAttachments", "Attachments", "Templates"
        };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RequestBodyInitializer> _logger;

        public RequestBodyInitializer(IHttpContextAccessor httpContextAccessor, ILogger<RequestBodyInitializer> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                var requestTelemetry = telemetry as RequestTelemetry;
                if (requestTelemetry == null)
                {
                    return;
                }

                if (_httpContextAccessor.HttpContext.Request.Method != HttpMethods.Post &&
                     _httpContextAccessor.HttpContext.Request.Method != HttpMethods.Put ||
                    !_httpContextAccessor.HttpContext.Request.Body.CanRead)
                {
                    return;
                }

                if (requestTelemetry.Properties.ContainsKey(HeaderConstants.RequestHeaderKeys.RequestJsonBody))
                {
                    return;
                }

                var isFileUploadRequest = FileUploadRequests.Any(_httpContextAccessor.HttpContext.Request.Path.ToString().Contains);

                var bodyString = _httpContextAccessor.HttpContext.GetRawBodyString(Encoding.UTF8);

                var sanitizedBody = isFileUploadRequest ? bodyString : SanitizeRequestBody(bodyString);

                if (!string.IsNullOrEmpty(sanitizedBody))
                {
                    requestTelemetry.Properties.Add(
                        HeaderConstants.RequestHeaderKeys.RequestJsonBody,
                        sanitizedBody);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error sending Telemetry to AppInsights");
            }
        }

        private static string SanitizeRequestBody(string bodyString)
        {
            if (string.IsNullOrWhiteSpace(bodyString))
            {
                return string.Empty;
            }

            var token = JToken.Parse(bodyString);
            var sanitizedToken = token.RemoveFields(PiiFields);

            return sanitizedToken.ToString();
        }
    }
}

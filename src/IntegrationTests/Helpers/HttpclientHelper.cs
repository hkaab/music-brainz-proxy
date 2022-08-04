using System;
using System.Net.Http;

namespace IntegrationTests.Helpers
{
    public static class HttpClientHelper
    {
        public static HttpClient WithHttpsBaseAddress(this HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost/");
            return httpClient;
        }
    }
}

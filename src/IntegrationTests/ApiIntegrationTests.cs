using AspNetCoreRateLimit;
using Common.Extensions;
using Domain;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MusicBrainzApi;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class ApiIntegrationTests
    {
        [Fact]
        public void WhenAppIsStarted_IHttpClientFactoryCreateMusicBrainzHttpClientSuccessfully()
        {
            using var factory = new WebApplicationFactory<Program>();

            var httpClientFactory = factory.Services.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("MusicBrainz");

            Assert.Equal("https://musicbrainz.org/ws/2/", httpClient.BaseAddress.ToString());
            Assert.Equal("Coles.Musicbrainz/1.0", httpClient.DefaultRequestHeaders.UserAgent.ToString());
        }

        [Fact]
        public void WhenAppIsStarted_IpRateLimitOptionsAreLoadedSuccessfully()
        {
            using var factory = new WebApplicationFactory<Program>();

            var options = factory.Services.GetRequiredService<IOptions<IpRateLimitOptions>>();
            var generalRule = options.Value.GeneralRules[0];

            Assert.Single(options.Value.GeneralRules);
            Assert.Equal("*", generalRule.Endpoint);
            Assert.Equal("1s", generalRule.Period);
            Assert.Equal(1, generalRule.Limit);
        }

        [Fact]
        public async Task WhenQueryArtistAsyncRequest_WithArtistName_Status200IsReturnedWithArtistCollection()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();
            var response = await httpClient.GetAsync("/Artist/QueryArtistAsync/jackson");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = Serialization.FromJson<ArtistCollection>(json);
            if (result != null)
            {
                Assert.True(result.Count > 1);
            }
            else
            {
                result = Serialization.FromJson<Artist>(json);
                Assert.True(result.Id != null);
            }
        }

        [Fact]
        public async Task WhenGetArtistByIdAsyncRequest_WithValidArtistId_Status200IsReturnedWithArtistDetails()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();

            var response = await httpClient.GetAsync("/Artist/GetArtistByIdAsync/6be2828f-6c0d-4059-99d4-fa18acf1a296");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = Serialization.FromJson<Artist>(json);

            Assert.IsType<Artist>(result);
            Assert.True(result.Releases != null && result.Releases.Count > 0);
        }

        [Fact]
        public async Task WhenGetArtistByIdAsyncRequest_WithNotExistedName_Status404NotFoundResult()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();
            var response = await httpClient.GetAsync("/Artist/GetArtistByIdAsync/fjkljdkljklfjklfj");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task WhenArtistRouteRequest_WithValidArtistId_Status200IsReturnedWithArtistDetails()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();

            var response = await httpClient.GetAsync("/Artist/6be2828f-6c0d-4059-99d4-fa18acf1a296");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = Serialization.FromJson<Artist>(json);

            Assert.IsType<Artist>(result);
            Assert.True(result.Releases != null && result.Releases.Count > 0);
        }

        [Fact]
        public async Task WhenGetArtistReleaseAsyncRequest_WithValidArtistId_Status200IsReturnedWithArtistDetails()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();

            var response = await httpClient.GetAsync("/Artist/GetArtistReleaseAsync/6be2828f-6c0d-4059-99d4-fa18acf1a296");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = Serialization.FromJson<ReleaseCollection>(json);

            Assert.IsType<ReleaseCollection>(result);
            Assert.True(result.Releases != null && result.Releases.Count > 0);
        }

        [Fact]
        public async Task WhenArtistReleaseRouteRequest_WithValidArtistId_Status200IsReturnedWithArtistDetails()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();

            var response = await httpClient.GetAsync("/Artist/Release/6be2828f-6c0d-4059-99d4-fa18acf1a296");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = Serialization.FromJson<ReleaseCollection>(json);

            Assert.IsType<ReleaseCollection>(result);
            Assert.True(result.Releases != null && result.Releases.Count > 0);
        }

        [Fact]
        public async Task WhenArtistReleaseRouteRequest_WithNotExistedName_Status404NotFoundResult()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();
            var response = await httpClient.GetAsync("/Artist/Release/fjkljdkljklfjklfj");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task WhenQueryArtistAsyncRequest_WithExactMatchArtistName_Status200IsReturnedWithArtistAndRelease()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();

            var response = await httpClient.GetAsync("/Artist/Query/janet%20%jackson");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = Serialization.FromJson<Artist>(json);

            Assert.IsType<Artist>(result);
            Assert.True(result.Releases != null && result.Releases.Count > 0);
        }

        [Fact]
        public async Task WhenQueryArtistAsyncRequest_WithNotExistedName_Status404NotFoundResult()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();
            var response = await httpClient.GetAsync("/Artist/Query/fgfdgfdgfdgfdg");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhenExceedRateLimit_ExpectException()
        {
            using var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient().WithHttpsBaseAddress();

            var requests = new List<string> { "1", "2", "3" };
            var allTasks = requests.Select(n => Task.Run(async () =>
            {
                var result = await httpClient.GetStringAsync("/Artist/Query/janet%20%jackson");

            })).ToList();

            async Task ConcurrentApiRequests() => await Task.WhenAll(allTasks);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => ConcurrentApiRequests());
            Assert.Equal("Response status code does not indicate success: 429 (Too Many Requests).", exception.Message);
        }

    }
}
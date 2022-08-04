using Domain;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.Builders
{
    public class ArtistServiceBuilder
    {
        private ArtistService _artistService;
        private object _returnedObject;
        public ArtistServiceBuilder()
        {
            _returnedObject = new();
        }
        public ArtistServiceBuilder ForGetArtistById()
        {
            _returnedObject = new Artist
            {
                Name = "Janet Jackson",
                Id = "12345-1222-233-33333",
                Releases = new List<Release>()
            };

            return this;
        }
        public ArtistServiceBuilder ForGetArtistReleaseAsync()
        {
            _returnedObject = new ReleaseCollection();

            return this;
        }
        public ArtistServiceBuilder ForQueryArtistAsyncWithNoResult()
        {
            _returnedObject = new ArtistCollection
            {
                Count = 0,
                Artists = new List<Artist> ()
            };
            return this;
        }

        public ArtistServiceBuilder ForQueryArtistAsyncWithSingleResult()
        {
            _returnedObject = new ArtistCollection
            {
                Count = 1,
                Artists = new List<Artist> {
                    new Artist { Gender = "Female", Id = "d51fad9c-5fda-4507-b258-c7ce4b435972", Name = "Janet Jackson", Rating = null, Releases = null }
                }
            };
            return this;
        }
        public ArtistServiceBuilder ForQueryArtistAsyncWithMultipleResult()
        {
            _returnedObject = new ArtistCollection
            {
                Count = 2,
                Artists = new List<Artist> {
                    new Artist { Gender = "Female", Id = "d51fad9c-5fda-4507-b258-c7ce4b435972", Name = "Janet Jackson", Rating = null, Releases = null},
                    new Artist { Gender = "Male", Id = "hkjhkjhkh-5fda-4507-b258-c7ce4b435972", Name = "Jim Jackson", Rating = null, Releases = null},
                }
            };
            return this;
        }
        public ArtistServiceBuilder ForQueryArtistAsyncWithMultipleResultWith100Score()
        {
            _returnedObject = new ArtistCollection
            {
                Count = 2,
                Artists = new List<Artist> {
                    new Artist { Gender = "Female", Id = "d51fad9c-5fda-4507-b258-c7ce4b435972", Name = "Janet Jackson", Rating = null, Releases = null,Score=100},
                    new Artist { Gender = "Male", Id = "hkjhkjhkh-5fda-4507-b258-c7ce4b435972", Name = "Jim Jackson", Rating = null, Releases = null},
                }
            };
            return this;
        }
        public ArtistService Build()
        {
            var json = JsonConvert.SerializeObject(_returnedObject);
            var fakeUrl = "https://musicbrainz.local/ws/2/";

            HttpResponseMessage httpResponse = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            Mock<HttpMessageHandler> mockHandler = new();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().StartsWith(fakeUrl)),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            HttpClient httpClient = new(mockHandler.Object)
            {
                BaseAddress = new Uri(fakeUrl)

            };
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient("MusicBrainz")).Returns(httpClient);
            _artistService = new ArtistService(mockHttpClientFactory.Object);

            return _artistService;
        }
    }
}

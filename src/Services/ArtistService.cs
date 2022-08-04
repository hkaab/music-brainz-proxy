using Domain;
using Interfaces;
using Common.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public class ArtistService : IArtistService
    {
        private const string QueryArtistTemplate = "artist/?query=\"{0}\"&limit={1}&offset={2}&fmt=json";
        private const string GetArtistTemplate = "artist/{0}?inc=releases&fmt=json";
        private const string GetReleaseTemplate = "release?artist={0}&fmt=json";
        private readonly HttpClient _httpClient;
        public ArtistService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MusicBrainz");
        }
        public async Task<Artist> GetArtistByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Artist Id must be specified.");
            }
            var uri = string.Format(GetArtistTemplate, id);

            var json = await Invoke(uri);
            return Serialization.FromJson<Artist>(json);
        }
        public async Task<ReleaseCollection> GetArtistReleaseAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Artist Id must be specified.");
            }
            var uri = string.Format(GetReleaseTemplate, id);

            var json = await Invoke(uri);

            return Serialization.FromJson<ReleaseCollection>(json);
        }
        public async Task<ArtistCollection> QueryArtistAsync(string name, int? limit = 100, int? offset = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Artist name must be specified.");
            }

            var uri = string.Format(QueryArtistTemplate, name, limit, offset);
            var json = await Invoke(uri);

            return Serialization.FromJson<ArtistCollection>(json);
        }

        private async Task<string> Invoke(string uri)
        {
            var response = await _httpClient.GetAsync(uri).ContinueWith((_) => _.Result.Content.ReadAsStringAsync().Result);
            return response;
        }

    }
}

using Domain;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IArtistService
    {
        Task<Artist> GetArtistByIdAsync(string id);
        Task<ReleaseCollection> GetArtistReleaseAsync(string id);
        Task<ArtistCollection> QueryArtistAsync(string id, int? limit = 100, int? offset=0);

    }
}

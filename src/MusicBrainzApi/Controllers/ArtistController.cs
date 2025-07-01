using Domain;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;

namespace MusicBrainzApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : BaseApiController
    {
        private readonly IArtistService _artistService;
        public ArtistController(IArtistService artistService, ILogger<ArtistController> logger, IRedisCache redisCache) : base(logger,redisCache)
        {
            _artistService = artistService;
        }

        [HttpGet]
        [Route("GetArtistByIdAsync/{artistId}")]
        [Route("{artistId}")]
        [ProducesResponseType(typeof(Artist), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArtistByIdAsync(string artistId)
        {
            try
            {
                if (string.IsNullOrEmpty(artistId))
                    return BadRequest("Artist Id must be specified.");

                var artist = await Cache.GetOrSetAsync(artistId, async () => await _artistService.GetArtistByIdAsync(artistId), CacheExpiry);

                if (artist == null || artist.Id == null)
                    return NotFound();

                return Ok(artist);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        [Route("GetArtistReleaseAsync/{artistId}")]
        [Route("Release/{artistId}")]
        [ProducesResponseType(typeof(ReleaseCollection),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArtistReleaseAsync(string artistId)
        {
            try
            {
                if (string.IsNullOrEmpty(artistId))
                    return BadRequest("Artist Id must be specified.");  

                var releases = await Cache.GetOrSetAsync(artistId, async () => await _artistService.GetArtistReleaseAsync(artistId), CacheExpiry);

                if (releases == null || releases.Releases == null)
                    return NotFound();

                return Ok(releases);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        [Route("QueryArtistAsync/{name}")]
        [Route("Query/{name}")]
        [ProducesResponseType(typeof(Artist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ArtistCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> QueryArtistAsync(string name, [FromQuery] int? limit = 50, [FromQuery]  int? offset = 0, [FromQuery] string sort="Name")
        {
            try
            {
               
                if (string.IsNullOrEmpty(name))
                    return BadRequest("Artist name must be specified.");

                var artist = await Cache.GetEntryAsync<Artist>(name);
                if (artist != null)
                    return Ok(artist);

                var artistCollection = await Cache.GetEntryAsync<ArtistCollection>(name);
                if (artistCollection != null)
                    return Ok(artistCollection);

                artistCollection = await _artistService.QueryArtistAsync(name, limit, offset);
                if (artistCollection == null || artistCollection.Count == 0)
                    return NotFound();

                artist = artistCollection.Artists.OrderByDescending(a => a.Score)
                                                           .Where(a => a.Score == 100)
                                                           .FirstOrDefault();
                if (artist != null)
                {
                    artist = await _artistService.GetArtistByIdAsync(artist.Id);
                    await Cache.SetEntryAsync(name, artist, CacheExpiry);
                    return Ok(artist);
                }


                if (artistCollection.Count == 1)
                {
                    artist = await _artistService.GetArtistByIdAsync(artistCollection.Artists[0].Id);
                    await Cache.SetEntryAsync(name, artist, CacheExpiry);
                    return Ok(artist);
                }

                artistCollection.Artists = artistCollection.Artists.OrderBy(sort).ToList();
                await Cache.SetEntryAsync(name, artistCollection, CacheExpiry);
                return Ok(artistCollection);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}

using Domain;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MusicBrainzApi.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

// <summary>
// Unit tests for the ArtistController class    
 
namespace UnitTests.Controllers
{
    public class ArtistControllerTests
    {
        private readonly Mock<IArtistService> _mockArtistService;
        private readonly Mock<ILogger<ArtistController>> _mockLogger;
        private readonly Mock<IRedisCache> _mockRedisCache;
        private readonly ArtistController _artistController;
        public ArtistControllerTests()
        {
            _mockArtistService = new Mock<IArtistService>();
            _mockLogger = new Mock<ILogger<ArtistController>>();
            _mockRedisCache = new Mock<IRedisCache>();
            _artistController = new ArtistController(_mockArtistService.Object, _mockLogger.Object, _mockRedisCache.Object);
        }

        [Fact]
        public async void GetArtistByIdAsync_WhenCalledWithValidParam_ReturnsArtist()
        {
            // Arrange
            var expected = new Artist { Id = "d51fad9c-5fda-4507-b258-c7ce4b435972" };

            _mockArtistService.Setup(service => service.GetArtistByIdAsync("d51fad9c-5fda-4507-b258-c7ce4b435972"))
                .Returns(Task.FromResult(expected));

            _mockRedisCache.Setup(cache => cache.GetOrSetAsync(
                It.IsAny<string>(), 
                It.IsAny<Func<Task<Artist>>>(), 
                It.IsAny<TimeSpan>())) //
                .Returns(Task.FromResult(expected));

            // Act
            var result = (ObjectResult)await _artistController.GetArtistByIdAsync("d51fad9c-5fda-4507-b258-c7ce4b435972");

            // Assert
            Assert.IsType<Artist>(result.Value);
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetArtistByIdAsync_WhenCalledWithValidParamWithNoResult_ReturnsNotFoundResult()
        {
            // Arrange
            var expected = new Artist();
            _mockArtistService.Setup(service => service.GetArtistByIdAsync("d51fad9c-5fda-4507-b258-c7ce4b435972")).Returns(Task.FromResult(expected));

            // Act
            var result = await _artistController.GetArtistByIdAsync("d51fad9c-5fda-4507-b258-c7ce4b435972");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void GetArtistByIdAsync_WhenCalledWithInvalidParam_ReturnsStatus400BadRequest()
        {
            // Arrange
            var expected = "Artist Id must be specified.";

            // Act
            var result = (ObjectResult)await _artistController.GetArtistByIdAsync(null);

            // Assert
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async void GetArtistByIdAsync_WhenException_ReturnsStatus500InternalServerError()
        {
            // Arrange
            var expected = "Internal Server Error";

            _mockRedisCache.Setup(cache => cache.GetOrSetAsync(
                It.IsAny<string>(), 
                It.IsAny<Func<Task<Artist>>>(), 
                It.IsAny<TimeSpan>()))
                .Throws(new Exception(expected));

            _mockArtistService.Setup(service => service.GetArtistByIdAsync("fakeid")).Throws(new Exception(expected));

            // Act
            var result = (ObjectResult)await _artistController.GetArtistByIdAsync("fakeid");

            // Assert
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public async void GetArtistReleaseAsync_WhenCalledWithValidParam_ReturnsReleaseCollection()
        {
            // Arrange
            var expected = new ReleaseCollection
            {
                Releases = new List<Release>()
            };

            _mockRedisCache.Setup(cache => cache.GetOrSetAsync(
                It.IsAny<string>(), 
                It.IsAny<Func<Task<ReleaseCollection>>>(), 
                It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult(expected));

            _mockArtistService.Setup(service => service.GetArtistReleaseAsync("d51fad9c-5fda-4507-b258-c7ce4b435972")).Returns(Task.FromResult(expected));

            // Act
            var result = (ObjectResult)await _artistController.GetArtistReleaseAsync("d51fad9c-5fda-4507-b258-c7ce4b435972");

            // Assert
            Assert.IsType<ReleaseCollection>(result.Value);
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetArtistReleaseAsync_WhenCalledWithValidParamWithNoResult_ReturnsNotFoundResult()
        {
            // Arrange
            var expected = new ReleaseCollection();

            _mockArtistService.Setup(service => service.GetArtistReleaseAsync("d51fad9c-5fda-4507-b258-c7ce4b435972")).Returns(Task.FromResult(expected));

            // Act
            var result = await _artistController.GetArtistReleaseAsync("d51fad9c-5fda-4507-b258-c7ce4b435972");

            // Assert   
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void GetArtistReleaseAsync_WhenCalledWithInvalidParam_ReturnsStatus400BadRequest()
        {
            // Arrange
            var expected = "Artist Id must be specified.";

            // Act
            var result = (ObjectResult)await _artistController.GetArtistReleaseAsync(null);

            // Assert
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async void GetArtistReleaseAsync_WhenException_ReturnsStatus500InternalServerError()
        {
            // Arrange
            var expected = "Internal Server Error";
            _mockRedisCache.Setup( service => service.GetOrSetAsync(
                It.IsAny<string>(), 
                It.IsAny<Func<Task<ReleaseCollection>>>(), 
                It.IsAny<TimeSpan>()))
                .Throws(new Exception(expected));

            _mockArtistService.Setup(service => service.GetArtistReleaseAsync("fakeid")).Throws(new Exception(expected));

            // Act
            var result = (ObjectResult)await _artistController.GetArtistReleaseAsync("fakeid");

            // Assert
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public async void QueryArtistAsync_WhenCalledWithValidParamAndCountGreaterThan1_ReturnsArtistCollection()
        {
            var expected = new ArtistCollection
            {
                Count = 2,
                Artists = new List<Artist>
                {
                    new Artist {Name="Janet Jackson"},
                    new Artist {Name= "Michael Jackson"}
                }
            };

            _mockArtistService.Setup(service => service.QueryArtistAsync("Jackson", 1, 0)).Returns(Task.FromResult(expected));
            var result = (ObjectResult)await _artistController.QueryArtistAsync("Jackson", 1, 0);

            Assert.IsType<ArtistCollection>(result.Value);
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void QueryArtistAsync_WhenCalledWithValidParamAndCountEqualTo1_ReturnsArtistWithReleases()
        {
            var returnedObject = new ArtistCollection
            {
                Count = 1,
                Artists = new List<Artist>
                {
                    new Artist {Name="Janet Jackson", Id="12345-1222-233-33333"}
                }
            };
            var expectedReleases = new List<Release>
            {
                new Release
                {
                    Id = "fake-release-id",
                    Quality = "awesome",
                    Title ="fake-title"
                }
            };
            var expected = new Artist
            {
                Name = "Janet Jackson",
                Id = "12345-1222-233-33333",
                Releases = expectedReleases
            };

            _mockArtistService.Setup(service => service.QueryArtistAsync("Janet Jackson", 1, 0)).Returns(Task.FromResult(returnedObject));
            _mockArtistService.Setup(service => service.GetArtistByIdAsync(returnedObject.Artists[0].Id)).Returns(Task.FromResult(expected));

            var result = (ObjectResult)await _artistController.QueryArtistAsync("Janet Jackson", 1, 0);

            Assert.IsType<Artist>(result.Value);
            Assert.Equal(expected, result.Value);
            Assert.Equal(expectedReleases, ((Artist)result.Value).Releases);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void QueryArtistAsync_WhenCalledWithValidParam_ReturnsNotFoundResult()
        {
            var expected = new ArtistCollection
            {
                Count = 0,
            };
            _mockArtistService.Setup(service => service.QueryArtistAsync("sdjsaldjdlksa", 1, 0)).Returns(Task.FromResult(expected));
            var result = await _artistController.QueryArtistAsync("sdjsaldjdlksa", 1, 0);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void QueryArtistAsync_WhenCalledWithInvalidParam_ReturnsStatus400BadRequest()
        {
            var expected = "Artist name must be specified.";
            _mockArtistService.Setup(service => service.QueryArtistAsync(null, 0, 0)).Throws(new ArgumentException(expected));
            var result = (ObjectResult)await _artistController.QueryArtistAsync(null, 0, 0);
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async void QueryArtistAsync_WhenException_ReturnsStatus500InternalServerError()
        {
            var expected = "Internal Server Error";
            _mockArtistService.Setup(service => service.QueryArtistAsync("nobody", 0, 0)).Throws(new Exception(expected));
            var result = (ObjectResult)await _artistController.QueryArtistAsync("nobody", 0, 0);
            Assert.Equal(expected, result.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

    }
}

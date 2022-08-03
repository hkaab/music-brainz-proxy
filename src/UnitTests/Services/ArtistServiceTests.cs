using Domain;
using Services;
using System;
using System.Collections.Generic;
using UnitTests.Builders;
using Xunit;

namespace UnitTests.Services
{
    public class ArtistServiceTests
    {
        public ArtistServiceTests()
        {
        }

        [Fact]
        public async void GetArtistByIdAsync_WhenCalledWithValidParam_ReturnsArtist()
        {
            ArtistService _artistService = new ArtistServiceBuilder().ForGetArtistById().Build();
            var result = await _artistService.GetArtistByIdAsync("d51fad9c-5fda-4507-b258-c7ce4b435972");
            Assert.IsType<Artist>(result);
        }

        [Fact]
        public async void GetArtistByIdAsync_WhenCalledWithInvalidParam_ThrowException()
        {
            ArtistService _artistService = new ArtistServiceBuilder().Build();
            await Assert.ThrowsAsync<ArgumentException>(() => _artistService.GetArtistByIdAsync(null));
        }

        [Fact]
        public async void GetArtistReleaseAsync_WhenCalledWithValidParam_ReturnsReleaseCollection()
        {
            ArtistService _artistService = new ArtistServiceBuilder().ForGetArtistReleaseAsync().Build();
            var result = await _artistService.GetArtistReleaseAsync("d51fad9c-5fda-4507-b258-c7ce4b435972");
            Assert.IsType<ReleaseCollection>(result);
        }

        [Fact]
        public async void GetArtistReleaseAsync_WhenCalledWithInvalidParam_ThrowException()
        {
            ArtistService _artistService = new ArtistServiceBuilder().Build();
            await Assert.ThrowsAsync<ArgumentException>(() => _artistService.GetArtistReleaseAsync(null));
        }

        [Fact]
        public async void QueryArtistAsync_WhenCalledWithValidParamWithMultipleResult_ReturnsArtistCollection()
        {
            ArtistService _artistService = new ArtistServiceBuilder().ForQueryArtistAsyncWithMultipleResult().Build();
            var result = await _artistService.QueryArtistAsync("Michael",10,0);
            Assert.IsType<ArtistCollection>(result);
        }
        [Fact]
        public async void QueryArtistAsync_WhenCalledWithValidParamWithSingleResult_ReturnsArtistCollection()
        {
            ArtistService _artistService = new ArtistServiceBuilder().ForQueryArtistAsyncWithSingleResult().Build();
            var result = await _artistService.QueryArtistAsync("Michael", 10, 0);
            Assert.IsType<ArtistCollection>(result);
        }
        [Fact]
        public async void QueryArtistAsync_WhenCalledWithValidParamWithNoResult_ReturnsArtistCollection()
        {
            ArtistService _artistService = new ArtistServiceBuilder().ForQueryArtistAsyncWithNoResult().Build();
            var result = await _artistService.QueryArtistAsync("Michael", 10, 0);
            Assert.IsType<ArtistCollection>(result);
        }

        [Fact]
        public async void QueryArtistAsync_WhenCalledWithInvalidParam_ThrowException()
        {
            ArtistService _artistService = new ArtistServiceBuilder().Build();
            await Assert.ThrowsAsync<ArgumentException>(() => _artistService.QueryArtistAsync(null,0,0));
        }

    }
}

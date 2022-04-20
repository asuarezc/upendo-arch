using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.Location;
using upendo.CrossCutting.Interfaces.Data.Rest;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Services.Helpers;
using upendo.Services.Logic;
using Xunit;

namespace upendeo.Services.Test
{
    public class LocationPageServiceTest
    {
        private static readonly ILocationPageService service = GetService();

        private static ILocationPageService GetService() => GetService(GetLocationServiceMock(), GetBasicAuthRestServiceMock());

        private static ILocationPageService GetService(ILocationService locationService, IBasicAuthRestService<ResultsResponse<Location>> basicAuthRestService) =>
            new LocationPageService(locationService, basicAuthRestService);

        private static ILocationService GetLocationServiceMock()
        {
            Mock<ILocationService> locationServiceMock = new();

            locationServiceMock
                .Setup(mock => mock.GetLocationAsync())
                .Returns(Task.FromResult(new Xamarin.Essentials.Location { Latitude = 0d, Longitude = 0d }));

            return locationServiceMock.Object;
        }

        private static IBasicAuthRestService<ResultsResponse<Location>> GetBasicAuthRestServiceMock()
        {
            Mock<IBasicAuthRestService<ResultsResponse<Location>>> basicAuthRestServiceMock = new();

            basicAuthRestServiceMock
                .Setup(mock => mock.GetAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultsResponse<Location> { Results = new List<Location> { new Location() }}));

            return basicAuthRestServiceMock.Object;
        }

        [Fact]
        public void InitServiceWithoutLocationServiceMock()
        {
            static ILocationPageService func() { return new LocationPageService(null, GetBasicAuthRestServiceMock()); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("locationService", exception.ParamName);
        }

        [Fact]
        public void InitServiceWithoutBasicAuthRestServiceMock()
        {
            static ILocationPageService func() { return new LocationPageService(GetLocationServiceMock(), null); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("basicAuthRestService", exception.ParamName);
        }

        [Fact]
        public async Task GetLocationAsync()
        {
            Location location = await service.GetLocationAsync();

            Assert.NotNull(location);
        }

        [Fact]
        public async Task GetLocationAsync_WhenLocationServiceReturnsNull()
        {
            Mock<ILocationService> locationServiceMock = new();

            locationServiceMock
                .Setup(mock => mock.GetLocationAsync())
                .Returns(Task.FromResult<Xamarin.Essentials.Location>(null));

            ILocationPageService locationPageService = GetService(locationServiceMock.Object, GetBasicAuthRestServiceMock());

            Location location = await locationPageService.GetLocationAsync();
            Assert.Null(location);
        }

        [Fact]
        public async Task GetLocationAsync_WhenBasicAuthRestServiceReturnsNull()
        {
            Mock<IBasicAuthRestService<ResultsResponse<Location>>> basicAuthRestServiceMock = new();

            basicAuthRestServiceMock
                .Setup(mock => mock.GetAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<ResultsResponse<Location>>(null));

            ILocationPageService locationPageService = GetService(GetLocationServiceMock(), basicAuthRestServiceMock.Object);

            Location location = await locationPageService.GetLocationAsync();
            Assert.Null(location);
        }
    }
}

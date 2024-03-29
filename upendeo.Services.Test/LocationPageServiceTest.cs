﻿using System;
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

        private static ILocationPageService GetService() => GetService(GetLocationServiceMock(), GetRestServiceFactoryMock());

        private static ILocationPageService GetService(ILocationService locationService, IRestServiceFactory restServiceFactory) =>
            new LocationPageService(locationService, restServiceFactory);

        private static ILocationService GetLocationServiceMock()
        {
            Mock<ILocationService> locationServiceMock = new();

            locationServiceMock
                .Setup(mock => mock.GetLocationAsync())
                .Returns(Task.FromResult(new Xamarin.Essentials.Location { Latitude = 0d, Longitude = 0d }));

            return locationServiceMock.Object;
        }

        private static IRestServiceFactory GetRestServiceFactoryMock()
        {
            Mock<IRestService> restServiceMock = new();
            Mock<IRestServiceFactory> restServiceFactoryMock = new();

            restServiceMock
                .Setup(mock => mock.GetAsync<ResultsResponse<Location>>(It.IsAny<Uri>()))
                .Returns(Task.FromResult(new ResultsResponse<Location> { Results = new List<Location> { new Location() }}));

            restServiceFactoryMock
                .Setup(mock => mock.GetBasicAuthRestService(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(restServiceMock.Object);

            return restServiceFactoryMock.Object;
        }

        [Fact]
        public void InitServiceWithoutLocationServiceMock()
        {
            static ILocationPageService func() { return new LocationPageService(null, GetRestServiceFactoryMock()); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("locationService", exception.ParamName);
        }

        [Fact]
        public void InitServiceWithoutRestServiceFactoryMock()
        {
            static ILocationPageService func() { return new LocationPageService(GetLocationServiceMock(), null); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("restServiceFactory", exception.ParamName);
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

            ILocationPageService locationPageService = GetService(locationServiceMock.Object, GetRestServiceFactoryMock());

            Location location = await locationPageService.GetLocationAsync();
            Assert.Null(location);
        }

        [Fact]
        public async Task GetLocationAsync_WhenBasicAuthRestServiceReturnsNull()
        {
            Mock<IRestService> restServiceMock = new();
            Mock<IRestServiceFactory> restServiceFactoryMock = new();

            restServiceMock
                .Setup(mock => mock.GetAsync<ResultsResponse<Location>>(It.IsAny<Uri>()))
                .Returns(Task.FromResult<ResultsResponse<Location>>(null));

            restServiceFactoryMock
                .Setup(mock => mock.GetBasicAuthRestService(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(restServiceMock.Object);

            ILocationPageService locationPageService = GetService(GetLocationServiceMock(), restServiceFactoryMock.Object);

            Location location = await locationPageService.GetLocationAsync();
            Assert.Null(location);
        }
    }
}

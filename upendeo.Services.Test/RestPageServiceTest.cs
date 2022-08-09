using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.Rest;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Services.Helpers;
using upendo.Services.Logic;
using Xunit;

namespace upendeo.Services.Test
{
    public class RestPageServiceTest
    {
        private static readonly IRestPageService service = new RestPageService(GetRestServiceFactoryMock());

        private static IRestServiceFactory GetRestServiceFactoryMock()
        {
            Mock<IRestService> restServiceMock = new();
            Mock<IRestServiceFactory> restServiceFactoryMock = new();

            restServiceMock
                .Setup(mock => mock.GetAsync<RestResponse<IEnumerable<User>>>(It.IsAny<Uri>()))
                .Returns(Task.FromResult(new RestResponse<IEnumerable<User>> { Data = new List<User> { new User() } }));

            restServiceFactoryMock
                .Setup(mock => mock.GetBasicAuthRestService(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(restServiceMock.Object);

            return restServiceFactoryMock.Object;
        }

        [Fact]
        public void InitServiceWithoutMock()
        {
            static IRestPageService func() { return new RestPageService(null); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("restServiceFactory", exception.ParamName);
        }

        [Fact]
        public async Task GetUsersAsync()
        {
            IEnumerable<User> users = await service.GetUsersAsync();

            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetUsersAsync_WhenBasicAuthRestServiceReturnsNull()
        {
            Mock<IRestService> restServiceMock = new();
            Mock<IRestServiceFactory> restServiceFactoryMock = new();

            restServiceMock
                .Setup(mock => mock.GetAsync<RestResponse<IEnumerable<User>>>(It.IsAny<Uri>()))
                .Returns(Task.FromResult<RestResponse<IEnumerable<User>>>(null));

            restServiceFactoryMock
                .Setup(mock => mock.GetBasicAuthRestService(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(restServiceMock.Object);

            IRestPageService restPageService = new RestPageService(restServiceFactoryMock.Object);

            IEnumerable<User> users = await restPageService.GetUsersAsync();

            Assert.Null(users);
        }
    }
}

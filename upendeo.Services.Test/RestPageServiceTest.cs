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
        private static readonly IRestPageService service = new RestPageService(GetBasicAuthRestServiceMock());

        private static IBasicAuthRestService<RestResponse<IEnumerable<User>>> GetBasicAuthRestServiceMock()
        {
            Mock<IBasicAuthRestService<RestResponse<IEnumerable<User>>>> basicAuthRestServiceMock = new();

            basicAuthRestServiceMock
                .Setup(mock => mock.GetAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new RestResponse<IEnumerable<User>> { Data = new List<User> { new User() } }));

            return basicAuthRestServiceMock.Object;
        }

        [Fact]
        public void InitServiceWithoutMock()
        {
            static IRestPageService func() { return new RestPageService(null); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("restService", exception.ParamName);
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
            Mock<IBasicAuthRestService<RestResponse<IEnumerable<User>>>> basicAuthRestServiceMock = new();

            basicAuthRestServiceMock
                .Setup(mock => mock.GetAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult<RestResponse<IEnumerable<User>>>(null));

            IRestPageService restPageService = new RestPageService(basicAuthRestServiceMock.Object);

            IEnumerable<User> users = await restPageService.GetUsersAsync();

            Assert.Null(users);
        }
    }
}

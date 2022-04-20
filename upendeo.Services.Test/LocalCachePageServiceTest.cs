using System;
using System.Collections.Generic;
using Moq;
using upendo.CrossCutting.Interfaces.Data.LocalCache;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Services.Logic;
using Xunit;

namespace upendeo.Services.Test
{
    public class LocalCachePageServiceTest
    {
        private static readonly ILocalCachePageService service = GetService();

        private static ILocalCachePageService GetService()
        {
            return new LocalCachePageService(GetCacheMock());
        }

        private static IMemoryCacheFactory GetCacheMock()
        {
            Mock<IMemoryCacheFactory> result = new();
            Mock<IMemoryCache<string>> cacheMock = new();

            cacheMock.Setup(mock => mock.GetAll()).Returns(new List<string> { new string("test") });
            cacheMock.Setup(mock => mock.ContainsKey(It.IsAny<string>())).Returns(false);
            cacheMock.Setup(mock => mock.ContainsKey("ContainsKey")).Returns(true);
            cacheMock.Setup(mock => mock.AddOrUpdate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            result.Setup(mock => mock.GetOrCreateCache<string>(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<float>(), It.IsAny<bool>())).Returns(cacheMock.Object);

            return result.Object;
        }

        [Fact]
        public void InitServiceWithoutMock()
        {
            static ILocalCachePageService func() { return new LocalCachePageService(null); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("memoryCacheManager", exception.ParamName);
        }

        [Fact]
        public void GetAllStrings()
        {
            IEnumerable<string> strings = service.GetAllStrings();

            Assert.NotNull(strings);
            Assert.NotEmpty(strings);
        }

        [Fact]
        public void AddString()
        {
            service.AddString("test", TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AddString_WhenItemIsNull()
        {
            static void action() => service.AddString(null, TimeSpan.FromSeconds(1));

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("item", exception.ParamName);
        }

        [Fact]
        public void AddString_WhenContainsKey()
        {
            service.AddString("ContainsKey", TimeSpan.FromSeconds(1));
        }
    }
}

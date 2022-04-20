using System.Collections.Generic;
using Moq;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;
using upendo.Services.Logic;
using Xunit;

namespace upendeo.Services.Test
{
    public class MainPageServiceTest
    {
        [Fact]
        public void GetMenuOptions()
        {
            Mock<ITranslationManager> localizationManagerMock = new();

            localizationManagerMock
                .Setup(mock => mock.GetResource(It.IsAny<StringKey>(), It.IsAny<string>()))
                .Returns(string.Empty);

            IMainPageService service = new MainPageService(localizationManagerMock.Object);
            IEnumerable<MenuOption> menuOptions = service.GetMenuOptions();

            Assert.NotNull(menuOptions);
            Assert.NotEmpty(menuOptions);
        }
    }
}

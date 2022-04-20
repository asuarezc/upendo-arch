using System;
using System.Collections.Generic;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Entities.Enums;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;

namespace upendo.Services.Logic
{
    public class MainPageService : IMainPageService
    {
        private readonly ITranslationManager localizationManager;

        public MainPageService(ITranslationManager localizationManager)
        {
            this.localizationManager = localizationManager ?? throw new ArgumentNullException(nameof(localizationManager));
        }

        public IEnumerable<MenuOption> GetMenuOptions(bool darkThem = false) => GetMenuOptionsInternal(darkThem);

        private IEnumerable<MenuOption> GetMenuOptionsInternal(bool darkThem = false)
        {
            return new List<MenuOption>
            {
                new MenuOption
                {
                    Description = localizationManager.GetResource(StringKey.menu_run_in_background),
                    OptionImage = "round_hourglass_empty_white_36",
                    GoImage = "round_arrow_forward_ios_black_36",
                    TopBarBackgroundHexColor = "#703c00",
                    OptionBackgroundHexColor = "#a36700",
                    NavigationPage = AvaliablePageFromMenu.RunBusy
                },
                new MenuOption
                {
                    Description = localizationManager.GetResource(StringKey.menu_rest_client),
                    OptionImage = "round_cloud_white_36",
                    GoImage = "round_arrow_forward_ios_black_36",
                    TopBarBackgroundHexColor = "#002f6c",
                    OptionBackgroundHexColor = "#01579b",
                    NavigationPage = AvaliablePageFromMenu.RestService
                },
                new MenuOption
                {
                    Description = localizationManager.GetResource(StringKey.menu_local_database),
                    OptionImage = "round_storage_white_36",
                    GoImage = "round_arrow_forward_ios_black_36",
                    TopBarBackgroundHexColor = "#142a00",
                    OptionBackgroundHexColor = "#345400",
                    NavigationPage = AvaliablePageFromMenu.LiteDB
                },
                new MenuOption
                {
                    Description = localizationManager.GetResource(StringKey.menu_local_memory_cache),
                    OptionImage = "round_memory_white_36",
                    GoImage = "round_arrow_forward_ios_black_36",
                    TopBarBackgroundHexColor = "#142a00",
                    OptionBackgroundHexColor = "#345400",
                    NavigationPage = AvaliablePageFromMenu.LocalMemoryChache
                },
                new MenuOption
                {
                    Description = localizationManager.GetResource(StringKey.menu_gps_location),
                    OptionImage = "round_place_white_36",
                    GoImage = "round_arrow_forward_ios_black_36",
                    TopBarBackgroundHexColor = "#870000",
                    OptionBackgroundHexColor = "#bf360c",
                    NavigationPage = AvaliablePageFromMenu.Location
                },
                new MenuOption
                {
                    Description = localizationManager.GetResource(StringKey.menu_settings),
                    OptionImage = "round_settings_white_36",
                    GoImage = "round_arrow_forward_ios_black_36",
                    TopBarBackgroundHexColor = "#333333",
                    OptionBackgroundHexColor = "#666666",
                    NavigationPage = AvaliablePageFromMenu.Settings
                }
            };
        }
    }
}

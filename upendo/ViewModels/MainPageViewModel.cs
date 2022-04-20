using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Autofac;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Models;
using System.Linq;
using Xamarin.Forms;
using System;
using upendo.ContentPages;
using upendo.Managers;
using upendo.Helpers;
using upendo.CrossCutting.Localization;
using upendo.CrossCutting.Entities.Enums;
using DryIoc;

namespace upendo.ViewModels
{
    public class MainPageViewModel : BaseViewModel<MainPageModel>
    {
        public override string Title => GetResource(StringKey.app_name);

        public override async Task<MainPageModel> GetInitialModelAsync()
        {
            MainPageModel model = null;

            try
            {
                model = await GetInitialModelInternalAsync();
            }
            catch (Exception ex)
            {
                ShowStatusMessage(GetResource(StringKey.global_error_loading_menu));
                ex.Log();
            }

            return model;
        }

        private async Task<MainPageModel> GetInitialModelInternalAsync()
        {
            MainPageModel model = new();

            await RunUnitOfWorkAsync(
                job: (IResolverContext scope) =>
                {
                    IMainPageService mainPageService = scope.Resolve<IMainPageService>();
                    return mainPageService.GetMenuOptions();
                },
                onSuccess: (IEnumerable<MenuOption> menuOptions) =>
                {
                    model.MenuOptions = new ObservableCollection<MenuOptionModel>(
                        from menuOption in menuOptions
                        select GetMenuOptionModelFrom(menuOption)
                    );
                }
            );

            return model;
        }

        private MenuOptionModel GetMenuOptionModelFrom(MenuOption menuOption)
        {
            ContentPage pageToNavigate = menuOption.NavigationPage switch
            {
                AvaliablePageFromMenu.RunBusy => new CameraAndGalleryPage(),
                AvaliablePageFromMenu.RestService => new RestPage(),
                AvaliablePageFromMenu.LiteDB => new LocalDBPage(),
                AvaliablePageFromMenu.LocalMemoryChache => new LocalCachePage(),
                AvaliablePageFromMenu.Location => new LocationPage(),
                AvaliablePageFromMenu.Settings => new SettingsPage(),
                _ => null,
            };

            MenuOptionModel result = new()
            {
                Description = menuOption.Description,
                TopBarBackgroundColor = Color.FromHex(menuOption.TopBarBackgroundHexColor),
                OptionBackgroundColor = Color.FromHex(menuOption.OptionBackgroundHexColor),
                OptionImageSource = ImageSource.FromFile(menuOption.OptionImage),
                GoImageSource = ImageSource.FromFile(menuOption.GoImage),
                Command = pageToNavigate != null
                    ? new Command(
                        () =>
                        {
                            TrackingManager.Instance.TrackEvent(
                                TrackingManager.TrakingEvent.MenuOptionTapped,
                                new Dictionary<string, string> {
                                    { "Tapped option", menuOption.NavigationPage.ToString() }
                                }
                            );

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Page.Navigation.PushAsync(pageToNavigate);
                            });
                        },
                        () => IsNotBusy)
                    : null
            };

            return result;
        }
    }
}

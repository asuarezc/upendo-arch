using System;
using System.Threading.Tasks;
using DryIoc;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public class LocationPageViewModel : BaseViewModel<LocationPageModel>
    {
        public override string Title => GetResource(StringKey.menu_gps_location);

        private Command getLocationCommand;
        public Command GetLocationCommand => getLocationCommand ??= new Command(
            async () => await SetCurrentLocationAsync(),
            () => IsNotBusy
        );

        private async Task SetCurrentLocationAsync()
        {
            await RunUnitOfWorkAsync(
                job: async (IResolverContext scope) =>
                {
                    ILocationPageService locationPageService = scope.Resolve<ILocationPageService>();
                    return await locationPageService.GetLocationAsync();
                },
                onSuccess: (Location location) =>
                {
                    if (location != null)
                        Model = new LocationPageModel(location);
                }
            );
        }
    }
}

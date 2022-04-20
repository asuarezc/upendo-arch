using upendo.Models;
using upendo.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace upendo.ContentPages
{
    public partial class LocationPage : LocationPageXaml
    {
        public LocationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(async () =>
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                    SendBackButtonPressed();
            });
        }
    }

    public class LocationPageXaml : BaseContentPage<LocationPageViewModel, LocationPageModel> { }
}

using System;
using System.Threading.Tasks;
using upendo.CrossCutting.Interfaces.Data.Location;
using Xamarin.Essentials;

namespace upendo.Services.Data.Location
{
    public class LocationService : ILocationService
    {
        public async Task<Xamarin.Essentials.Location> GetLocationAsync()
        {
            if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted)
                return null;

            Xamarin.Essentials.Location lastKnownLocation = await Geolocation.GetLastKnownLocationAsync();

            //Not null and not too old
            if (lastKnownLocation != null && lastKnownLocation.Timestamp <= DateTime.UtcNow.AddMinutes(5))
                return lastKnownLocation;

            return await Geolocation.GetLocationAsync(new GeolocationRequest(
                GeolocationAccuracy.Default,
                TimeSpan.FromSeconds(1))
            );
        }
    }
}

using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.Location;
using upendo.CrossCutting.Interfaces.Data.Rest;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Services.Helpers;

namespace upendo.Services.Logic
{
    public class LocationPageService : ILocationPageService
    {
        private static readonly string apiKey = "27032018ff0b44878332dea8b0b1fdd9";
        private readonly ILocationService locationService;
        private readonly IRestService restService;

        public LocationPageService(ILocationService locationService, IRestServiceFactory restServiceFactory)
        {
            if (restServiceFactory == null)
                throw new ArgumentNullException(nameof(restServiceFactory));

            this.locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            restService = restServiceFactory.GetBasicAuthRestService("test", "test");
        }

        public async Task<Location> GetLocationAsync()
        {
            Xamarin.Essentials.Location location = await locationService.GetLocationAsync();

            if (location == null)
                return null;

            string url = $"https://api.geoapify.com/v1/geocode/reverse?lat={location.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={location.Longitude.ToString(CultureInfo.InvariantCulture)}&format=json&apiKey={apiKey}";
            ResultsResponse<Location> reverseLocations = await restService.GetAsync<ResultsResponse<Location>>(new Uri(url));

            return reverseLocations != null && reverseLocations.Results != null && reverseLocations.Results.Any()
                ? reverseLocations.Results.First()
                : null;
        }
    }
}

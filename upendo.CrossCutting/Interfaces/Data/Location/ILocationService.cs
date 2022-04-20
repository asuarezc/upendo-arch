using System;
using System.Threading.Tasks;

namespace upendo.CrossCutting.Interfaces.Data.Location
{
    public interface ILocationService
    {
        Task<Xamarin.Essentials.Location> GetLocationAsync();
    }
}

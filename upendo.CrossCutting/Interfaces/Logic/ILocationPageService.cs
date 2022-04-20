using System;
using System.Threading.Tasks;
using upendo.CrossCutting.Entities;

namespace upendo.CrossCutting.Interfaces.Logic
{
    public interface ILocationPageService
    {
        Task<Location> GetLocationAsync();
    }
}

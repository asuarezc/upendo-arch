using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using upendo.CrossCutting.Entities;

namespace upendo.CrossCutting.Interfaces.Logic
{
    public interface IRestPageService
    {
        Task<IEnumerable<User>> GetUsersAsync();
    }
}

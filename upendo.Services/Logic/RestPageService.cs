using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.Rest;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Services.Helpers;

namespace upendo.Services.Logic
{
    public class RestPageService : IRestPageService
    {
        private readonly IRestService restService;

        public RestPageService(IRestServiceFactory restServiceFactory)
        {
            if (restServiceFactory == null)
                throw new ArgumentNullException(nameof(restServiceFactory));

            restService = restServiceFactory.GetBasicAuthRestService("Test", "Test");
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            RestResponse<IEnumerable<User>> result = await restService.GetAsync<RestResponse<IEnumerable<User>>>(new Uri(@"https://reqres.in/api/users?page=2"));

            return result != null && result.Data != null && result.Data.Any()
                ? result.Data
                : null;
        }
    }
}

using System;

namespace upendo.CrossCutting.Interfaces.Data.Rest
{
    public interface IRestServiceFactory
    {
        int CurrentAlive { get; }

        IRestService GetBasicAuthRestService(string username, string password);

        IRestService GetTokenBasedRestService(string token);
    }
}

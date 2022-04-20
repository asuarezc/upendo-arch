using System.Collections.Generic;
using System.Linq;
using System.Threading;
using upendo.CrossCutting.Interfaces.Data.Rest;

namespace upendo.Services.Data.Rest
{
    public sealed class RestServiceFactory : IRestServiceFactory
    {
        private readonly ReaderWriterLockSlim locker = new(LockRecursionPolicy.NoRecursion);
        private readonly List<IRestService> services = new();

        private int currentAlive;
        public int CurrentAlive
        {
            get
            {
                locker.EnterReadLock();

                try
                {
                    return currentAlive;
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }
        }

        public IRestService GetBasicAuthRestService(string username, string password)
        {
            IRestService result;

            RemovedAlreadyExpiredOrDisposed();

            locker.EnterReadLock();

            try
            {
                result = services.FirstOrDefault(service => service.ServiceType == IRestService.RestServiceType.BasicAuth);
            }
            finally
            {
                locker.ExitReadLock();
            }

            if (result != null)
                return result;

            result = new BasicAuthRestService(username, password);

            locker.EnterWriteLock();

            try
            {
                services.Add(result);
                currentAlive++;
            }
            finally
            {
                locker.ExitWriteLock();
            }

            return result;
        }

        public IRestService GetTokenBasedRestService(string token)
        {
            IRestService result;

            RemovedAlreadyExpiredOrDisposed();

            locker.EnterReadLock();

            try
            {
                result = services.FirstOrDefault(service => service.ServiceType == IRestService.RestServiceType.TokenBased);
            }
            finally
            {
                locker.ExitReadLock();
            }

            if (result != null)
                return result;

            result = new TokenBasedRestService(token);

            locker.EnterWriteLock();

            try
            {
                services.Add(result);
                currentAlive++;
            }
            finally
            {
                locker.ExitWriteLock();
            }

            return result;
        }

        private void RemovedAlreadyExpiredOrDisposed()
        {
            locker.EnterUpgradeableReadLock();

            try
            {
                List<IRestService> itemsToRemove = services
                    .Where(service =>
                        service == null
                        || (
                            service is TokenBasedRestService tokenBasedRestService)
                            && tokenBasedRestService.Expired
                        )
                    ?.ToList();

                foreach (IRestService restService in itemsToRemove)
                {
                    services.Remove(restService);
                    restService?.Dispose();
                    currentAlive--;
                }  
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }
    }
}

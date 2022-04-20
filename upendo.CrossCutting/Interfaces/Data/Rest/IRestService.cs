using System;
using System.Threading.Tasks;

namespace upendo.CrossCutting.Interfaces.Data.Rest
{
    /// <summary>
    /// Do not worry about disposing, IRestServiceFactory will manage instances for you
    /// </summary>
    public interface IRestService : IDisposable
    {
        public enum RestServiceType
        {
            BasicAuth,
            TokenBased
        }

        TimeSpan Timeout { get; set; }
        RestServiceType ServiceType { get; }

        Task<T> GetAsync<T>(Uri uri) where T : class;
        Task<T> PostAsync<T>(Uri uri, object requestBody) where T : class;
        Task<T> PutAsync<T>(Uri uri, object requestBody) where T : class;
        Task DeleteAsync(Uri uri);
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;
using upendo.CrossCutting.Interfaces.Data.Rest;

namespace upendo.Services.Data.Rest
{
    public class TokenBasedRestService : IRestService
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(2);
        private static readonly string tokenHeaderName = "token";
        private HttpClient httpClient;
        private bool disposedValue;
        private TimeSpan timeout;

        public TimeSpan Timeout
        {
            get => timeout;
            set
            {
                if (value <= TimeSpan.Zero)
                    throw new InvalidOperationException($"\"{nameof(Timeout)}\" property cannot be lower or equals TimeSpan.Zero");

                timeout = value;
                httpClient?.Dispose();

                httpClient = new HttpClient { Timeout = value };
            }
        }

        public string Token { get; private set; }

        public IRestService.RestServiceType ServiceType => IRestService.RestServiceType.TokenBased;

        public bool Expired { get; private set; }

        public TokenBasedRestService(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            Token = token;
            Timeout = defaultTimeout;
        }

        public async Task<T> GetAsync<T>(Uri uri) where T : class
        {
            ThrowIfUriIsNull(uri);

            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.GetAsync(uri);
            }
            finally
            {
                RemoveTokenRequestHeaderIfPresent();
            }

            ThrowIfResponseIsInvalid(response);
            RestHelper.ThrowIfResponseContentIsNull(response);

            return await RestHelper.GetResultFromValidResponseAsync<T>(response);
        }

        public async Task<T> PostAsync<T>(Uri uri, object requestBody) where T : class
        {
            return await GetResult<T>(RestHelper.HttpMethod.Post, uri, requestBody);
        }

        public async Task<T> PutAsync<T>(Uri uri, object requestBody) where T : class
        {
            return await GetResult<T>(RestHelper.HttpMethod.Put, uri, requestBody);
        }

        public async Task DeleteAsync(Uri uri)
        {
            ThrowIfUriIsNull(uri);

            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.DeleteAsync(uri);
            }
            finally
            {
                RemoveTokenRequestHeaderIfPresent();
            }

            ThrowIfResponseIsInvalid(response);
        }

        private async Task<T> GetResult<T>(RestHelper.HttpMethod method, Uri uri, object requestBody) where T : class
        {
            ThrowIfUriIsNull(uri);
            ThrowIfRequestBodyIsNull(requestBody);

            HttpResponseMessage response = null;

            try
            {
                switch (method)
                {
                    case RestHelper.HttpMethod.Post:
                        response = await httpClient.PostAsync(uri, RestHelper.GetStringBodyContent(requestBody));
                        break;

                    case RestHelper.HttpMethod.Put:
                        response = await httpClient.PutAsync(uri, RestHelper.GetStringBodyContent(requestBody));
                        break;
                }
            }
            finally
            {
                RemoveTokenRequestHeaderIfPresent();
            }

            ThrowIfResponseIsInvalid(response);
            RestHelper.ThrowIfResponseContentIsNull(response);

            return await RestHelper.GetResultFromValidResponseAsync<T>(response);
        }

        private void ThrowIfUriIsNull(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
        }

        private void ThrowIfRequestBodyIsNull(object requestBody)
        {
            if (requestBody == null)
                throw new ArgumentNullException(nameof(requestBody));
        }

        private void RemoveTokenRequestHeaderIfPresent()
        {
            if (httpClient.DefaultRequestHeaders.Contains(tokenHeaderName))
                httpClient.DefaultRequestHeaders.Remove(tokenHeaderName);
        }

        private void ThrowIfResponseIsInvalid(HttpResponseMessage response)
        {
            try
            {
                RestHelper.ThrowIfResponseIsInvalid(response);
            }
            catch (Exception ex) when (!string.IsNullOrEmpty(ex.Message) && ex.Message == "Unauthorized")
            {
                Expired = true;
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                httpClient?.Dispose();

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

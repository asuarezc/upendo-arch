using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using upendo.CrossCutting.Interfaces.Data.Rest;

namespace upendo.Services.Data.Rest
{
    public class BasicAuthRestService : IRestService
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(2);
        private static readonly string basicAuthHeaderName = "basic";
        private HttpClient httpClient;
        private bool disposedValue;
        private TimeSpan timeout;

        public string Username { get; private set; }

        public string Password { get; private set; }

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

        public IRestService.RestServiceType ServiceType => IRestService.RestServiceType.BasicAuth;

        public BasicAuthRestService(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            Timeout = defaultTimeout;
        }

        public async Task<T> GetAsync<T>(Uri uri) where T : class
        {
            ThrowIfUriIsNull(uri);
            AddBasicAuthHeader();

            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.GetAsync(uri);
            }
            finally
            {
                RemoveBasicAuthHeaderIfPresent();
            }

            RestHelper.ThrowIfResponseIsInvalid(response);
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
            AddBasicAuthHeader();

            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.DeleteAsync(uri);
            }
            finally
            {
                RemoveBasicAuthHeaderIfPresent();
            }

            RestHelper.ThrowIfResponseIsInvalid(response);
        }

        private async Task<T> GetResult<T>(RestHelper.HttpMethod method, Uri uri, object requestBody) where T : class
        {
            ThrowIfUriIsNull(uri);
            ThrowIfRequestBodyIsNull(requestBody);
            AddBasicAuthHeader();

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
                RemoveBasicAuthHeaderIfPresent();
            }

            RestHelper.ThrowIfResponseIsInvalid(response);
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

        private void AddBasicAuthHeader()
        {
            string base64AuthHeaderValue = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{Username}:{Password}")
            );

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                basicAuthHeaderName, base64AuthHeaderValue
            );
        }

        private void RemoveBasicAuthHeaderIfPresent()
        {
            if (httpClient.DefaultRequestHeaders.Authorization != null)
                httpClient.DefaultRequestHeaders.Authorization = null;
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

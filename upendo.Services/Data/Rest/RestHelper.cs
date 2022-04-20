using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace upendo.Services.Data.Rest
{
    internal static class RestHelper
    {
        internal enum HttpMethod
        {
            Post,
            Put
        }

        internal static async Task<T> GetResultFromValidResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            string responseJsonBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseJsonBody))
                throw new HttpRequestException("Response content body is null or empty");

            return JsonConvert.DeserializeObject<T>(responseJsonBody);
        }

        internal static StringContent GetStringBodyContent(object requestBody)
        {
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

            if (string.IsNullOrEmpty(jsonRequestBody))
                throw new InvalidOperationException($"Serialized json body request from {nameof(requestBody)} is null or empty");

            StringContent requestBodyContent = new(jsonRequestBody, Encoding.UTF8, "application/json");

            if (requestBodyContent == null)
                throw new InvalidOperationException($"String body content from serialized json request from {nameof(requestBody)} is null");

            return requestBodyContent;
        }

        internal static void ThrowIfResponseIsInvalid(HttpResponseMessage response)
        {
            if (response == null)
                throw new HttpRequestException("Null response from service");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.Unauthorized)
                    throw new HttpRequestException($"Response status code is \"{response.StatusCode} = {(int)response.StatusCode}\"");
                else
                    throw new HttpRequestException("Unauthorized");
            }
        }

        internal static void ThrowIfResponseContentIsNull(HttpResponseMessage response)
        {
            if (response.Content == null)
                throw new HttpRequestException("Response content is null");
        }
    }
}

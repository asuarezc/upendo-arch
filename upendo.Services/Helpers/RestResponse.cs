using System;
using Newtonsoft.Json;

namespace upendo.Services.Helpers
{
    public class RestResponse<T>
    {
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}

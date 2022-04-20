using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace upendo.Services.Helpers
{
    public class ResultsResponse<T>
    {
        [JsonProperty(PropertyName = "results")]
        public IEnumerable<T> Results { get; set; }
    }
}

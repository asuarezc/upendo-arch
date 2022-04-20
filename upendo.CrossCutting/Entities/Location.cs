using System;
using Newtonsoft.Json;

namespace upendo.CrossCutting.Entities
{
    public class Location
    {
        [JsonProperty(PropertyName = "address_line1")]
        public string AddressLineOne { get; set; }

        [JsonProperty(PropertyName = "address_line2")]
        public string AddressLineTwo { get; set; }
    }
}

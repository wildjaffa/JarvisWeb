using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models.NOAA
{
    public class Forecast
    {
        [JsonProperty("@context")]
        public List<object> Context { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }
}

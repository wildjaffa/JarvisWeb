using Newtonsoft.Json;

namespace JarvisWeb.Services.Models.NOAA
{
    public class Elevation
    {
        [JsonProperty("unitCode")]
        public string UnitCode { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}

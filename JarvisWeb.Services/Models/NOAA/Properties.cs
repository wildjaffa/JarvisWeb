using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models.NOAA
{
    public class Properties
    {
        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("forecastGenerator")]
        public string ForecastGenerator { get; set; }

        [JsonProperty("generatedAt")]
        public DateTime GeneratedAt { get; set; }

        [JsonProperty("updateTime")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("validTimes")]
        public string ValidTimes { get; set; }

        [JsonProperty("elevation")]
        public Elevation Elevation { get; set; }

        [JsonProperty("periods")]
        public List<Period> Periods { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models.NOAA
{
    public class Period
    {
        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("isDaytime")]
        public bool IsDaytime { get; set; }

        [JsonProperty("temperature")]
        public int Temperature { get; set; }

        [JsonProperty("temperatureTrend")]
        public string TemperatureTrend { get; set; }

        [JsonProperty("probabilityOfPrecipitation")]
        public ProbabilityOfPrecipitation ProbabilityOfPrecipitation { get; set; }

        [JsonProperty("windSpeed")]
        public string WindSpeed { get; set; }

        [JsonProperty("windDirection")]
        public string WindDirection { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("shortForecast")]
        public string ShortForecast { get; set; }

        [JsonProperty("detailedForecast")]
        public string DetailedForecast { get; set; }
    }
}

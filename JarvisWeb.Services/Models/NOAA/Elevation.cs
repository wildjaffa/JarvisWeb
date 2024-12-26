using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

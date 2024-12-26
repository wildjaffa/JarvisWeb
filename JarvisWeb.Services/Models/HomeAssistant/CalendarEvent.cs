using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models.HomeAssistant
{
    public class CalendarEvent
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("start")]
        public Start Start { get; set; }

        [JsonProperty("end")]
        public End End { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }
}

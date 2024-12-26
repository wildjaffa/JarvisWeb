using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models.HomeAssistant
{
    public class End
    {
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }
    }
}

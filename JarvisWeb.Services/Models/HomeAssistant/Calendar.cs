using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models.HomeAssistant
{
    public class Calendar
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

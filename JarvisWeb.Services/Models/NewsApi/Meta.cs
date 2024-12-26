using Newtonsoft.Json;

namespace JarvisWeb.Services.Models.NewsApi
{
    public class Meta
    {
        [JsonProperty("found")]
        public int Found { get; set; }

        [JsonProperty("returned")]
        public int Returned { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }
    }
}

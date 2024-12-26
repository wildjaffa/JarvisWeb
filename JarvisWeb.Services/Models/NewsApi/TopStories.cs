using Newtonsoft.Json;

namespace JarvisWeb.Services.Models.NewsApi;

public class TopStories
{
    [JsonProperty("meta")]
    public Meta Meta { get; set; }

    [JsonProperty("data")]
    public List<Datum> Data { get; set; }
}


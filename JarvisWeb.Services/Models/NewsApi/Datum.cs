using Newtonsoft.Json;

namespace JarvisWeb.Services.Models.NewsApi;


public class Datum
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("keywords")]
    public string Keywords { get; set; }

    [JsonProperty("snippet")]
    public string Snippet { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("categories")]
    public List<string> Categories { get; set; }

    [JsonProperty("relevance_score")]
    public object RelevanceScore { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; }
}
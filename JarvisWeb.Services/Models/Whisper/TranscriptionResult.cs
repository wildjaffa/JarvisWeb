using Newtonsoft.Json;

namespace JarvisWeb.Services.Models.Whisper;

public class TranscriptionResult
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("segments")]
    public List<Segment> Segments { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }
}

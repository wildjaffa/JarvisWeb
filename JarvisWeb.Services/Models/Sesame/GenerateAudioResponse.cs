using Newtonsoft.Json;

namespace JarvisWeb.Services.Models.Sesame;

public class GenerateAudioResponse
{
    [JsonProperty("audio_file")]
    public string AudioFilePath { get; set; }
}

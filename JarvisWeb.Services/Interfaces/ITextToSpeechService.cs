using JarvisWeb.Services.Models;

public interface ITextToSpeechService
{
    void Dispose();
    Task Initialize();
    Task<ServiceResponseModel<string>> ConvertTextToSpeech(string text, string voiceId, string outputFormat, string audioFilePath);
    Task TearDown();
}
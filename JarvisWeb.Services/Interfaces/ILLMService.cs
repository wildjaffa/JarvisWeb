using JarvisWeb.Services.Models;
using JarvisWeb.Services.Models.Ollama;

namespace JarvisWeb.Services.Interfaces
{
    public interface ILLMService
    {
        Task<ServiceResponseModel<LLMCompletion>> GetLLMCompletionAsync(
            string prompt,
            string? model = null
        );

        IAsyncEnumerable<ChatResponse> StreamLLMCompletionAsync(
            string prompt,
            string? model = null
        );
    }
}

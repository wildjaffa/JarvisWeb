using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using JarvisWeb.Services.Models.Ollama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace JarvisWeb.Services.Adapters.LLM;

public class OLLamaService(ILogger<OLLamaService> logger, IConfiguration configuration)
    : ILLMService
{
    private readonly ILogger<OLLamaService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ServiceResponseModel<LLMCompletion>> GetLLMCompletionAsync(
        string prompt,
        string? model = null
    )
    {
        try
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return new ServiceResponseModel<LLMCompletion>
                {
                    IsSuccess = false,
                    ErrorMessage = "Prompt is empty.",
                };
            }
            var modelName = model ?? _configuration["OLLama:Model"];
            if (string.IsNullOrEmpty(modelName))
            {
                return new ServiceResponseModel<LLMCompletion>
                {
                    IsSuccess = false,
                    ErrorMessage = "Model name is not configured.",
                };
            }
            var baseUrl = _configuration["OLLama:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new ServiceResponseModel<LLMCompletion>
                {
                    IsSuccess = false,
                    ErrorMessage = "Base URL is not configured.",
                };
            }
            var options = new RestClientOptions(baseUrl);
            var client = new RestClient(options);
            var request = new RestRequest("api/chat", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var requestBody = new ChatRequest
            {
                Model = modelName,
                Messages = [new Message { Role = "user", Content = prompt }],
                Stream = false,
            };

            var requestBodyJson = JsonConvert.SerializeObject(requestBody);

            request.AddParameter("application/json", requestBodyJson, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(response.Content!);
            var completion = new LLMCompletion { Completion = chatResponse!.Message.Content };

            return new ServiceResponseModel<LLMCompletion> { Data = completion, IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new ServiceResponseModel<LLMCompletion>
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public async IAsyncEnumerable<ChatResponse> StreamLLMCompletionAsync(
        string prompt,
        string? model = null
    )
    {
        if (string.IsNullOrEmpty(prompt))
        {
            yield break;
        }

        var modelName = model ?? _configuration["OLLama:Model"];
        if (string.IsNullOrEmpty(modelName))
        {
            yield break;
        }

        var baseUrl = _configuration["OLLama:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            yield break;
        }

        var options = new RestClientOptions(baseUrl);
        var client = new RestClient(options);
        var request = new RestRequest("api/chat", Method.Post);
        request.AddHeader("Content-Type", "application/json");

        var requestBody = new ChatRequest
        {
            Model = modelName,
            Messages = [new Message { Role = "user", Content = prompt }],
            Stream = true,
        };

        var requestBodyJson = JsonConvert.SerializeObject(requestBody);
        request.AddParameter("application/json", requestBodyJson, ParameterType.RequestBody);

        using var response = await client.DownloadStreamAsync(request, CancellationToken.None);

        using var reader = new StreamReader(response);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return JsonConvert.DeserializeObject<ChatResponse>(line);
        }
    }
}

using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using JarvisWeb.Services.Models.Ollama;
using Newtonsoft.Json;
using RestSharp;

namespace JarvisWeb.Services.Adapters.LLM
{
    public class OLLamaService : ILLMService
    {
        public async Task<ServiceResponseModel<LLMCompletion>> GetLLMCompletionAsync(string prompt)
        {
            try
            {
                var options = new RestClientOptions("http://PrimaryLinux:11434")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("api/chat", Method.Post);
                request.AddHeader("Content-Type", "text/plain");

                var requestBody = new ChatRequest
                {
                    Model = "llama3.2",
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            Role = "user",
                            Content = prompt,
                        },
                    },
                    Stream = false,
                };

                var requestBodyJson = JsonConvert.SerializeObject(requestBody);

                request.AddParameter("text/plain", requestBodyJson, ParameterType.RequestBody);
                RestResponse response = await client.ExecuteAsync(request);
                var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(response.Content);
                var completion = new LLMCompletion
                {
                    Completion = chatResponse.Message.Content,
                };

                return new ServiceResponseModel<LLMCompletion>
                {
                    Data = completion,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseModel<LLMCompletion>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}

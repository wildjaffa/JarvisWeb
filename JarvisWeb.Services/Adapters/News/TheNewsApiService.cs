using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using JarvisWeb.Services.Models.NewsApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Adapters.News
{
    public class TheNewsApiService(ILogger<TheNewsApiService> logger, IConfiguration configuration) : INewsService
    {
        private readonly ILogger<TheNewsApiService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ServiceResponseModel<IEnumerable<NewsStory>>> GetNewsAsync(DateTime startDate, DateTime endDate, int limit)
        {
            try
            {
                _logger.LogInformation("Getting news from The News API");

                var apiKey = _configuration["TheNewsApi:ApiKey"];

                var options = new RestClientOptions("https://api.thenewsapi.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest($"v1/news/top?api_token={apiKey}&locale=us&limit=3", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);
                var stories = JsonConvert.DeserializeObject<TopStories>(response.Content);
                var newsStories = stories.Data.Select(a => new NewsStory
                {
                    Title = a.Title,
                    Content = a.Snippet,
                    Description = a.Description,
                    Source = a.Source,
                    Url = a.Url
                });

                var result = new ServiceResponseModel<IEnumerable<NewsStory>>
                {
                    Data = newsStories,
                    IsSuccess = true,
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news from The News API");
                return new ServiceResponseModel<IEnumerable<NewsStory>>
                {
                    Data = [],
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                };
            }
        }
    }
}

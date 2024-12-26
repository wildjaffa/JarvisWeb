using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Services
{
    public class SummaryGenerationService(ILogger<SummaryGenerationService> logger, 
        INewsService newsService,
        IWeatherService weatherService,
        ICalendarService calendarService,
        ILLMService llmService)
    {
        private readonly ILogger<SummaryGenerationService> _logger = logger;
        private readonly INewsService _newsService = newsService;
        private readonly IWeatherService _weatherService = weatherService;
        private readonly ICalendarService _calendarService = calendarService;
        private readonly ILLMService _llmService = llmService;

        private const string _beginPrompt = "You are a helpful executive assistant. I am your manager. My name is Josh Jensen. You may refer to me in a variety of ways including, sir, Mr. Jensen, Mr. Josh, or other business friendly ways, be creative with your wording. Do not include any information not included in the dataset you are provided. I need a morning summary with the following information:";

        public async Task<string> GenerateSummaryAsync(SummaryGenerationRequest request)
        {
            _logger.LogInformation("Generating summary for {0}", request);

            //var storiesTask = _newsService.GetNewsAsync(DateTime.Now.AddDays(-1), DateTime.Now, 3);
            var eventsTask = _calendarService.GetEventsAsync(DateTime.Now.AddDays(-3), DateTime.Now.AddDays(1));
            var weatherTask = _weatherService.GetWeatherAsync(DateTime.Now, DateTime.Now.AddDays(1));

            await Task.WhenAll(//storiesTask, 
                eventsTask, weatherTask);
            //var stories = storiesTask.Result;
            var events = eventsTask.Result;
            var weather = weatherTask.Result;

            var promptData = new PromptData();
            
            ProcessWeather(promptData, weather);

            ProccessEvents(promptData, events);

            //ProcessNews(promptData, stories);

            promptData.NotesFromYesterday = "I wasn't able to get the summary generation service quite done as of yet, so focus on getting the video generating then worry about getting it to show on the screen.";

            var serializedPromptData = JsonConvert.SerializeObject(promptData);

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine(_beginPrompt);
            promptBuilder.AppendLine(serializedPromptData);

            var aiSummary = await _llmService.GetLLMCompletionAsync(promptBuilder.ToString());

            if (!aiSummary.IsSuccess)
            {
                return $"There was an error generating the summary: {aiSummary.ErrorMessage}";
            }

            return aiSummary.Data.Completion;
        }

        private static void ProcessWeather(PromptData promptData, ServiceResponseModel<IEnumerable<WeatherForecast>> weather)
        {
            if (!weather.IsSuccess)
            {
                promptData.EveningWeatherSummary = $"There was an error getting the weather: {weather.ErrorMessage}";
                return;
            }
            var forecast = weather.Data.ToArray();
            promptData.MorningWeatherSummary = forecast[0].Summary;
            promptData.EveningWeatherSummary = forecast[1].Summary;
            promptData.PercentChanceOfRain = forecast[0].ChanceOfRain;
        }

        private static void ProccessEvents(PromptData promptData, ServiceResponseModel<IEnumerable<CalendarEvent>> events)
        {
            if (!events.IsSuccess)
            {
                return;
            }
            var eventList = events.Data.ToArray();
            promptData.MeetingCount = eventList.Length;
            var minutes = eventList.Sum(e => e.EndDate.Subtract(e.StartDate).TotalMinutes);
            promptData.TotalMeetingTimeInHours = Math.Round((double)(minutes / 60), 1);
        }

        private static void ProcessNews(PromptData promptData, ServiceResponseModel<IEnumerable<NewsStory>> stories)
        {
            if (!stories.IsSuccess)
            {
                return;
            }
            promptData.NewsStories = stories.Data.Select(s => new NewsSummaryData
            {
                Title = s.Title,
                Description = s.Description,
                Source = s.Source,
            }).ToList();
        }
    }
}

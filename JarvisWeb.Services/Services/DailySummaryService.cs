using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace JarvisWeb.Services.Services
{
    public class DailySummaryService(
        ILogger<DailySummaryService> logger,
        JarvisWebDbContext context,
        INewsService newsService,
        IWeatherService weatherService,
        ICalendarService calendarService,
        ILLMService llmService,
        IVideoGenerationService videoGenerationService) : BaseService<DailySummary>(context, logger, context.DailySummaries)
    {
        private readonly INewsService _newsService = newsService;
        private readonly IWeatherService _weatherService = weatherService;
        private readonly ICalendarService _calendarService = calendarService;
        private readonly ILLMService _llmService = llmService;
        private readonly IVideoGenerationService _videoGenerationService = videoGenerationService;


        private const string _beginPrompt = "You are a helpful executive assistant. I am your manager. My name is Josh Jensen. You may refer to me in a variety of ways including, sir, Mr. Jensen, Mr. Josh, or other business friendly ways, be creative with your wording. Do not include any information not included in the dataset you are provided. I need a morning summary with the following information:";

        public async Task<DailySummary> GenerateSummaryAsync(SummaryGenerationRequest request)
        {
            _logger.LogInformation("Generating summary for {0}", request);

            var storiesTask = _newsService.GetNewsAsync(DateTime.Now.AddDays(-1), DateTime.Now, 3);
            var eventsTask = _calendarService.GetEventsAsync(DateTime.Now.AddDays(0), DateTime.Now.AddDays(1));
            var weatherTask = _weatherService.GetWeatherAsync(DateTime.Now, DateTime.Now.AddDays(1));

            await Task.WhenAll(storiesTask,
                eventsTask, weatherTask);
            var stories = storiesTask.Result;
            var events = eventsTask.Result;
            var weather = weatherTask.Result;

            var dailySummary = new DailySummary
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Date = DateTime.Now,
            };

            var promptData = new PromptData();

            ProcessWeather(promptData, weather);

            ProcessEvents(promptData, events);

            ProcessNews(promptData, stories);
            
            var lastDailySummary = await GetLatestDailySummary(request.UserId);

            var endOfDayNote = await _context.EndOfDayNotes
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefaultAsync(e => e.UserId == request.UserId);

            promptData.NotesFromYesterday = "No notes from yesterday";
            if (endOfDayNote != null && lastDailySummary?.EndOfDayNoteId != endOfDayNote.Id)
            {
                promptData.NotesFromYesterday = endOfDayNote.Note;
                dailySummary.EndOfDayNoteId = endOfDayNote.Id;
            }
            else
            {
                dailySummary.EndOfDayNoteId = null;
            }

            var serializedPromptData = JsonConvert.SerializeObject(promptData);

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine(_beginPrompt);
            promptBuilder.AppendLine(serializedPromptData);

            var aiSummary = await _llmService.GetLLMCompletionAsync(promptBuilder.ToString());

            if (!aiSummary.IsSuccess)
            {
                dailySummary.SummaryText = $"There was an error generating the summary: {aiSummary.ErrorMessage}";
            }
            else
            {
                dailySummary.SummaryText = aiSummary.Data.Completion;
            }

            var savedSummary = await Create(dailySummary);

            if(request.GenerateVideo)
            {
                await GenerateSummaryVideo(request.UserId, savedSummary.Id);
            }

            return savedSummary;
        }

        public async Task GenerateSummaryVideo(Guid userId, Guid summaryId)
        {
            var summary = await _context.DailySummaries.FirstOrDefaultAsync(d => d.UserId == userId && d.Id == summaryId);
            if (summary == null)
            {
                return;
            }
            var generationResult = await _videoGenerationService.GenerateVideoFromText(summary.SummaryText, summary.Id.ToString());
            if (!generationResult.IsSuccess)
            {
                return;
            }
            summary.SummaryVideoPath = generationResult.Data;
            await Update(summary);
        }

        public async Task<DailySummary?> GetLatestDailySummary(Guid userId)
        {
           return await _context.DailySummaries
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(e => e.UserId == userId);
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

        private static void ProcessEvents(PromptData promptData, ServiceResponseModel<IEnumerable<CalendarEvent>> events)
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

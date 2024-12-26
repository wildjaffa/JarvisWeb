using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Adapters.Calendar
{
    public class HomeAssistantCalendarService(ILogger<HomeAssistantCalendarService> logger, IConfiguration configuration) : ICalendarService
    {
        private readonly ILogger<HomeAssistantCalendarService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ServiceResponseModel<IEnumerable<CalendarEvent>>> GetEventsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {

                var calendarEntityIds = _configuration["HomeAssistant:CalendarEntityIds"];
                var calendarEntityIdList = calendarEntityIds?.Split(',').Select(i => i.Trim());

                if (calendarEntityIdList == null || !calendarEntityIdList.Any())
                {
                    return new ServiceResponseModel<IEnumerable<CalendarEvent>>
                    {
                        Data = [],
                        IsSuccess = false,
                        ErrorMessage = "No calendar entity IDs found in configuration",
                    };
                }

                var calendarEvents = new List<CalendarEvent>();
                foreach (var calendarEntityId in calendarEntityIdList)
                {
                    var events = await GetCalendarEventsForCalendar(calendarEntityId, startDate, endDate);
                    calendarEvents.AddRange(events);
                }

                return new ServiceResponseModel<IEnumerable<CalendarEvent>>
                {
                    Data = calendarEvents,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar events from Home Assistant");
                return new ServiceResponseModel<IEnumerable<CalendarEvent>>
                {
                    Data = [],
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                };
            }
        }

        private async Task<IEnumerable<CalendarEvent>> GetCalendarEventsForCalendar(string calendarEntityId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var baseUrl = _configuration["HomeAssistant:BaseUrl"];
                var token = _configuration["HomeAssistant:ApiKey"];
                var path = $"api/calendars/{calendarEntityId}";

                var query = $"start={startDate.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)}&end={endDate.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)}";
                var options = new RestClientOptions(baseUrl)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest($"{path}?{query}", Method.Get);
                request.AddHeader("Authorization", $"Bearer {token}");
                RestResponse response = await client.ExecuteAsync(request);
                var calendarEvents = JsonConvert.DeserializeObject<Models.HomeAssistant.CalendarEvent[]>(response.Content);
                return calendarEvents?
                    .Select(a => new CalendarEvent
                    {
                        Title = a.Summary,
                        StartDate = a.Start.DateTime,
                        EndDate = a.End.DateTime,
                        Description = a.Description,
                    }) ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar events from Home Assistant");
                return [];
            }
        }
    }
}

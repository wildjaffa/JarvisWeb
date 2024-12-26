using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Adapters.Weather
{
    public class NOAAWeatherService(ILogger<NOAAWeatherService> logger, IConfiguration configuration) : IWeatherService
    {
        private readonly ILogger<NOAAWeatherService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ServiceResponseModel<IEnumerable<WeatherForecast>>> GetWeatherAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var gridX = _configuration["NOAA:GridPoins:X"];
                var gridY = _configuration["NOAA:GridPoins:Y"];
                var forecastOfficeId = _configuration["NOAA:ForecastOfficeId"];
                var units = _configuration["NOAA:Units"];
                var options = new RestClientOptions("https://api.weather.gov")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest($"gridpoints/{forecastOfficeId}/{gridX},{gridY}/forecast?units={units}", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);
                var forecast = JsonConvert.DeserializeObject<Models.NOAA.Forecast>(response.Content);
                var weatherForecasts = forecast?.Properties.Periods.Select(a => new WeatherForecast
                {
                    Name = a.Name,
                    Date = a.StartTime,
                    TemperatureF = a.Temperature,
                    Summary = a.DetailedForecast,
                    WindSpeed = a.WindSpeed,
                    ChanceOfRain = a.ProbabilityOfPrecipitation.Value ?? 0,
                }).Where(f => f.Date > startDate && f.Date < endDate);

                var result = new ServiceResponseModel<IEnumerable<WeatherForecast>>
                {
                    Data = weatherForecasts,
                    IsSuccess = true,
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather from NOAA");
                return new ServiceResponseModel<IEnumerable<WeatherForecast>>
                {
                    Data = [],
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                };
            }
        }
    }
}

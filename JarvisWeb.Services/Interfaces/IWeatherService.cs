using JarvisWeb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<ServiceResponseModel<IEnumerable<WeatherForecast>>> GetWeatherAsync(DateTime startDate, DateTime endDate);
    }
}

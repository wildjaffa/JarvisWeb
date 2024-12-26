using JarvisWeb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Interfaces
{
    public interface ICalendarService
    {
        Task<ServiceResponseModel<IEnumerable<CalendarEvent>>> GetEventsAsync(DateTime startDate, DateTime endDate);
    }
}

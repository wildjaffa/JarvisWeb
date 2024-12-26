using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models
{
    public class PromptData
    {
        public int MeetingCount { get; set; }
        public double TotalMeetingTimeInHours { get; set; }
        public string MorningWeatherSummary { get; set; }
        public int PercentChanceOfRain { get; set; }
        public string EveningWeatherSummary { get; set; }
        public List<NewsSummaryData> NewsStories { get; set; }
        public string NotesFromYesterday { get; set; }
    }

    public class NewsSummaryData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
    }
}

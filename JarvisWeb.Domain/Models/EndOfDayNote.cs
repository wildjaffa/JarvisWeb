namespace JarvisWeb.Domain.Models
{
    public class EndOfDayNote : UserData
    {
        public DateTime DateTime { get; set; }
        public string Note { get; set; }
        public string? AudioFilePath { get; set; }

        public DailySummary DailySummary { get; set; }
    }
}

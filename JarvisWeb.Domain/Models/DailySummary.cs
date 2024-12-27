using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Domain.Models
{
    public class DailySummary : UserData
    {
        public DateTime Date { get; set; }
        public string SummaryText { get; set; }
        public string? SummaryVideoPath { get; set; }
        public Guid? EndOfDayNoteId { get; set; }
        public EndOfDayNote EndOfDayNote { get; set; }
    }
}

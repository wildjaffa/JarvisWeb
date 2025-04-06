using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JarvisWeb.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Nicknames { get; set; }
        public bool IsInOffice { get; set; }
        public Guid? LastSeenDailySummaryId { get; set; }

        [JsonIgnore]
        public virtual List<EndOfDayNote> EndOfDayNotes { get; set; }
        [JsonIgnore]
        public virtual List<DailySummary> DailySummaries { get; set; }
        [JsonIgnore]
        public virtual List<ApiKey> ApiKeys { get; set; }

    }
}

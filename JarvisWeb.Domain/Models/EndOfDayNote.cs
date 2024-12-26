namespace JarvisWeb.Domain.Models
{
    public class EndOfDayNote
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Note { get; set; }
        public Guid UserId { get; set; }
    }
}

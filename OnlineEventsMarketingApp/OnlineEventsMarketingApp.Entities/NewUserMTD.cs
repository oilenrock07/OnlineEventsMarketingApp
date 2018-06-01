using System.ComponentModel.DataAnnotations;

namespace OnlineEventsMarketingApp.Entities
{
    public class NewUserMTD
    {
        [Key]
        public int ID { get; set; }

        public string Inhouse { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ActualCount { get; set; }
    }
}

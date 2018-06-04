using System.ComponentModel.DataAnnotations;

namespace OnlineEventsMarketingApp.Entities
{
    public class NewUserMTD
    {
        [Key]
        public int ID { get; set; }

        public int TMCode { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}

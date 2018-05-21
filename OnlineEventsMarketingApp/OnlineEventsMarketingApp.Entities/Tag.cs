using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineEventsMarketingApp.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public string TagName { get; set; }
        public short Month { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineEventsMarketingApp.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        [StringLength(50)]
        public string TagName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

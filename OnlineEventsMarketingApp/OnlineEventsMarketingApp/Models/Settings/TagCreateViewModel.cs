using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OnlineEventsMarketingApp.Models.Settings
{
    public class TagCreateViewModel
    {
        public int TagId { get; set; }

        [Required]
        [StringLength(50)]
        public string TagName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public IEnumerable<SelectListItem> Months { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }
}
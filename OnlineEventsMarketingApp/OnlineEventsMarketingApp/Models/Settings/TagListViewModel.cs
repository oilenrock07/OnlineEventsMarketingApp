using System;
using System.Collections.Generic;
using OnlineEventsMarketingApp.Entities;
using System.Web.Mvc;

namespace OnlineEventsMarketingApp.Models.Settings
{
    public class TagListViewModel
    {
        public DateTime Test { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }
}
using System;

namespace OnlineEventsMarketingApp.Models.Settings
{
    public class TagCreateViewModel
    {
        public string TagName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Models.Reports
{
    public class WeeklyTagRunViewModel
    {
        public IEnumerable<WeeklyReportDTO> WeeklyReport { get; set; }

        public IEnumerable<WeeklyInhouseSummaryDTO> WeeklyInhouseSummary { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }

        public int  Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }

        public int GetWeeklyInhouseCount(int tagId, string inhouse)
        {
            if (WeeklyInhouseSummary != null)
            {
                var tag = WeeklyInhouseSummary.FirstOrDefault(x => x.TagId == tagId && x.InHouse == inhouse);
                if (tag != null)
                    return tag.Count;
            }

            return 0;
        }

        public int GetWeeklyInhouseTotal(string inhouse)
        {
            if (WeeklyInhouseSummary != null)
                return WeeklyInhouseSummary.Where(x => x.InHouse == inhouse).Sum(x => x.Count);

            return 0;
        }

        public int GetWeeklyInhouseOverAllTotal(int tagId)
        {
            if (WeeklyInhouseSummary != null)
                return WeeklyInhouseSummary.Where(x => x.TagId == tagId).Sum(x => x.Count);

            return 0;
        }

        public int GetWeeklyInhouseTotalCount()
        {
            if (WeeklyInhouseSummary != null)
                return WeeklyInhouseSummary.Sum(x => x.Count);

            return 0;
        }
    }
}

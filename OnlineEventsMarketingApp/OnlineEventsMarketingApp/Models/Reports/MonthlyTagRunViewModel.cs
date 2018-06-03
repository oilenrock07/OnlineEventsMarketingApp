using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineEventsMarketingApp.Models.Reports
{
    public class MonthlyTagRunViewModel
    {
        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }

        public IEnumerable<MonthlyReportData> InhouseMonthlyReport { get; set; }
        public IEnumerable<MonthlyReportData> OnlineMonthlyReport { get; set; }
    }
}
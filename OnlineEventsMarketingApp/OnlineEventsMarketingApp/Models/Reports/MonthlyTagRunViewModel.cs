using System.Collections.Generic;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Services.DataTransferObjects;

namespace OnlineEventsMarketingApp.Models.Reports
{
    public class MonthlyTagRunViewModel
    {
        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
        public IEnumerable<MonthlyConsultationACTDTO> MonthlyConsultations { get; set; }
    }
}
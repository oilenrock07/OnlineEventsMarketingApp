using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Entities;

namespace OnlineEventsMarketingApp.Models.Data
{
    public class DataSheetViewModel
    {
        [DataType(DataType.Upload)]
        public string File { get; set; }

        public IEnumerable<DataSheet> DataSheets { get; set; }

        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }

        public int Month { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
    }
}
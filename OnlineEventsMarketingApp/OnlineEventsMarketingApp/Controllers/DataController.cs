using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Common.Helpers;
using OnlineEventsMarketingApp.Models.Data;

namespace OnlineEventsMarketingApp.Controllers
{
    public class DataController : Controller
    {
        public DataController()
        {
            
        }

        [Authorize]
        public ActionResult DataSheet()
        {
            var now = DateTime.Now;

            var viewModel = new DataSheetViewModel
            {
                Year = now.Year,
                Month = now.Month,
                Years = GetYearList(),
                Months = GetMonthList()
            };
            return View(viewModel);
        }

        private IEnumerable<SelectListItem> GetMonthList()
        {
            var months = MonthYearHelper.GetMonths().Select(x => new SelectListItem
            {
                Text = x.Item2,
                Value = x.Item1.ToString()
            });

            return months;
        }

        private IEnumerable<SelectListItem> GetYearList()
        {
            var months = MonthYearHelper.GetYears().Select(x => new SelectListItem
            {
                Text = x.ToString(),
                Value = x.ToString()
            });

            return months;
        }
    }
}
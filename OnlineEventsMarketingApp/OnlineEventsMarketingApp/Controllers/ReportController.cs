using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IDataSheetService _dataSheetService;

        public ReportController(IDataSheetService dataSheetService)
        {
            _dataSheetService = dataSheetService;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult WeeklyTagsRun()
        {
            var data = _dataSheetService.GetWeeklyReport(5, 2018);
            return View();
        }
    }
}
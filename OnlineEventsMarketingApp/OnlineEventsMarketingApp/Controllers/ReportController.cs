using System;
using System.Linq;
using System.Web.Mvc;
using MoreLinq;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Helpers;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Models.Reports;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IDataSheetService _dataSheetService;
        private readonly ITagService _tagService;
         
        public ReportController(IDataSheetService dataSheetService, ITagService tagService)
        {
            _dataSheetService = dataSheetService;
            _tagService = tagService;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult WeeklyTagsRun(int? month = null, int? year = null)
        {
            var viewModel = new WeeklyTagRunViewModel
            {
                Month = month ?? DateTime.Now.Month,
                Year = year ?? DateTime.Now.Year,
                Months = MonthYearHelper.GetMonthList(),
                Years = MonthYearHelper.GetYearList()
            };

            var data = _dataSheetService.GetWeeklyReport(viewModel.Month, viewModel.Year);
            var inhouseSummary = _dataSheetService.GetWeeklyInHouseSummary(viewModel.Month, viewModel.Year);

            viewModel.WeeklyReport = data;
            viewModel.WeeklyInhouseSummary = inhouseSummary;
            viewModel.Tags = _tagService.GetTags(viewModel.Year, viewModel.Month).OrderBy(x => x.StartDate);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult MonthlyTagsRun(int? year = null)
        {
            var viewModel = new MonthlyTagRunViewModel
            {
                Year = year ?? DateTime.Now.Year,
                Years = MonthYearHelper.GetYearList()
            };

            viewModel.MonthlyConsultations = _dataSheetService.GetMonthlyConsultationReport(viewModel.Year);
            return View(viewModel);
        }
    }
}
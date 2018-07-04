using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MoreLinq;
using OnlineEventsMarketingApp.Common.Extensions;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Helpers;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Models.Reports;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
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
        public ActionResult WeeklyTagsRun(int? year = null, int? month= null)
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
        public ActionResult MonthlyTagsRun(int? year = null, string months = null)
        {
            var monthList = MonthYearHelper.GetMonthList();
            var selectedMonth = !String.IsNullOrEmpty(months) ? months.Split(',') : new string [0];
            var selectedMonthList = !String.IsNullOrEmpty(months) ? selectedMonth : monthList.Select(x => x.Value);

            var viewModel = new MonthlyTagRunViewModel
            {                                
                Year = year ?? DateTime.Now.Year,
                SelectedMonths = selectedMonthList,
                Months = monthList,
                Years = MonthYearHelper.GetYearList()
            };

            var onlineList = new List<MonthlyReportData>();
            var inHouseList = new List<MonthlyReportData>();
            GetMonthlyReportData(viewModel.Year, onlineList, inHouseList);

            viewModel.InhouseMonthlyReport = inHouseList;
            viewModel.OnlineMonthlyReport = onlineList;
            return View(viewModel);
        }

        public void ExportWeeklyReportToExel(int year, int month)
        {
            var data = _dataSheetService.GetWeeklyReport(month, year);
            var inhouseSummary = _dataSheetService.GetWeeklyInHouseSummary(month, year);
            var tags = _tagService.GetTags(year, month).OrderBy(x => x.StartDate);

            var reportHelper = new ReportHelper();
            var list = new List<ExportDataSourceBase>
            {
                reportHelper.GenerateWeeklyDataTable(data, tags),
                reportHelper.GenerateWeeklySummaryDataTable(inhouseSummary, tags)
            };

            var fileName = String.Format("Weekly Tags Run for {0} {1}", new DateTime(year, month, 1).ToString("MMMM"), year);
            Export.ToExcel(Response, list, fileName);
        }

        public void ExportMonthlyReportToExcel(int? year = null, string months = null)
        {
            //var monthList = MonthYearHelper.GetMonthList();
            //var selectedMonth = !String.IsNullOrEmpty(months) ? months.Split(',') : new string[0];
            //var selectedMonthList = !String.IsNullOrEmpty(months) ? selectedMonth : monthList.Select(x => x.Value);
            var selectedYear = year ?? DateTime.Now.Year;

            var onlineList = new List<MonthlyReportData>();
            var inHouseList = new List<MonthlyReportData>();
            GetMonthlyReportData(selectedYear, onlineList, inHouseList);

            var reportHelper = new ReportHelper();
            var list = new List<ExportDataSourceBase>
            {
                reportHelper.GenerateMonthlyDataTable(inHouseList, Common.Constants.Constants.INHOUSE),
                reportHelper.GenerateMonthlyDataTable(onlineList, Common.Constants.Constants.ONLINE)
            };

            //var selectedMonthNames = selectedMonthList.Select(x => new DateTime(selectedYear, x.ToInt(), 1).ToString("MMMM"));
            var fileName = String.Format("Monthly Tags Report for {0}", year);

            Export.ToExcel(Response, list, fileName);           
        }

        private void GetMonthlyReportData(int year, IList<MonthlyReportData> onlineList, IList<MonthlyReportData> inHouseList)
        {
            var monthlyRunsCount = _dataSheetService.GetMonthlyRunsCount(year);
            var monthlyConsultations = _dataSheetService.GetMonthlyConsultationReport(year);
            var monthlyNewUsers = _dataSheetService.GetMonthlyNewUserReport(year);

            foreach (var month in MonthYearHelper.GetMonths())
            {
                //inhouse
                inHouseList.Add(new MonthlyReportData
                {
                    Month = month.Item1,
                    MonthName = month.Item2,
                    Inhouse = Common.Constants.Constants.INHOUSE,
                    NoOfRuns = GetMonthlyRunsCount(monthlyRunsCount, month.Item1, Common.Constants.Constants.INHOUSE),
                    ConsultationACT = GetMonthlyConsultationACTCount(monthlyConsultations, month.Item1, Common.Constants.Constants.INHOUSE),
                    NUACT = GetMonthlyNewUserCount(monthlyNewUsers, month.Item1, Common.Constants.Constants.INHOUSE)
                });

                //online
                onlineList.Add(new MonthlyReportData
                {
                    Month = month.Item1,
                    MonthName = month.Item2,
                    Inhouse = Common.Constants.Constants.INHOUSE,
                    NoOfRuns = GetMonthlyRunsCount(monthlyRunsCount, month.Item1, Common.Constants.Constants.ONLINE),
                    ConsultationACT = GetMonthlyConsultationACTCount(monthlyConsultations, month.Item1, Common.Constants.Constants.ONLINE),
                    NUACT = GetMonthlyNewUserCount(monthlyNewUsers, month.Item1, Common.Constants.Constants.ONLINE)
                });
            }
        }

        private int GetMonthlyRunsCount(IEnumerable<MonthlyRunsCountDTO> monthlyRunsCount, int month, string type)
        {
            var runsCount = monthlyRunsCount.FirstOrDefault(x => x.Month == month && x.InHouse == type);
            return runsCount != null ? runsCount.Count : 0;
        }

        private int GetMonthlyConsultationACTCount(IEnumerable<MonthlyConsultationACTDTO> monthlyConsultationCount, int month, string type)
        {
            var consultationCount = monthlyConsultationCount.FirstOrDefault(x => x.Month == month && x.Inhouse == type);
            return consultationCount != null ? consultationCount.ACT : 0;
        }

        private int GetMonthlyNewUserCount(IEnumerable<NewUserMTDDTO> monthlyNewUserCount, int month, string type)
        {
            var newUserCount = monthlyNewUserCount.FirstOrDefault(x => x.Month == month && x.InHouse == type);
            return newUserCount != null ? newUserCount.ActualCount : 0;
        }
    }
}
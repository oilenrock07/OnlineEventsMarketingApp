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

            var monthlyRunsCount = _dataSheetService.GetMonthlyRunsCount(viewModel.Year);
            var monthlyConsultations = _dataSheetService.GetMonthlyConsultationReport(viewModel.Year);
            var monthlyNewUsers = _dataSheetService.GetMonthlyNewUserReport(viewModel.Year);

            var onlineList = new List<MonthlyReportData>();
            var inHouseList = new List<MonthlyReportData>();
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

            viewModel.InhouseMonthlyReport = inHouseList;
            viewModel.OnlineMonthlyReport = onlineList;
            return View(viewModel);
        }

        public void ExportMonthlyReportToExcel(int? year = null, string months = null)
        {
            var monthList = MonthYearHelper.GetMonthList();
            var selectedMonth = !String.IsNullOrEmpty(months) ? months.Split(',') : new string[0];
            var selectedMonthList = !String.IsNullOrEmpty(months) ? selectedMonth : monthList.Select(x => x.Value);

            var selectedYear = year ?? DateTime.Now.Year;
            var monthlyRunsCount = _dataSheetService.GetMonthlyRunsCount(selectedYear);
            var monthlyConsultations = _dataSheetService.GetMonthlyConsultationReport(selectedYear);
            var monthlyNewUsers = _dataSheetService.GetMonthlyNewUserReport(selectedYear);

            var onlineList = new List<MonthlyReportData>();
            var inHouseList = new List<MonthlyReportData>();
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

            var selectedMonthNames = selectedMonthList.Select(x => new DateTime(selectedYear, x.ToInt(), 1).ToString("MMMM"));
            var fileName = String.Format("Monthly Tags Report for {0} {1}", String.Join(",", selectedMonthNames), year);

            var dt = new DataTable();

            var tgtConsultation = new DataColumn()
            {
                ColumnName = "TGT Consultation",
                Caption = "TGT",
                DataType = typeof (int)
            };
            var tgtNU = new DataColumn()
            {
                ColumnName = "TGT NU",
                Caption = "TGT",
                DataType = typeof(int)
            };
            dt.Columns.Add("Month", typeof(string));
            dt.Columns.Add("# Of Runs", typeof(int));
            dt.Columns.Add(tgtConsultation);
            dt.Columns.Add("ACT Consultation", typeof(int));
            dt.Columns.Add("ACT vs TGT % Consultation", typeof(string));
            dt.Columns.Add(tgtNU);
            dt.Columns.Add("ACT NU", typeof(int));
            dt.Columns.Add("ACT vs TGT % NU", typeof(string));

            foreach (var item in inHouseList)
            {
                var row = dt.NewRow();
                row["Month"] = item.MonthName;
                row["# Of Runs"] = item.NoOfRuns;
                row["TGT Consultation"] = item.ConsultationTGT;
                row["ACT Consultation"] = item.ConsultationACT;
                row["ACT vs TGT % Consultation"] = item.ConsultationACTVsTGT;
                row["TGT NU"] = item.NUTGT;
                row["ACT NU"] = item.NUACT;
                row["ACT vs TGT % NU"] = item.NUACTVsTGT;

                dt.Rows.Add(row);
            }

            Export.ToExcel(Response, dt, fileName, OnRowCreated);
        }

        private void OnRowCreated(object sender, GridViewRowEventArgs gridViewRowEventArgs)
        {
            if (gridViewRowEventArgs.Row.RowType == DataControlRowType.Header) // If header created
            {
                var gridview = (GridView)sender;

                // Creating a Row
                GridViewRow HeaderRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);

                //Adding Year Column
                TableCell HeaderCell = new TableCell();
                HeaderCell.Text = "";
                HeaderCell.ColumnSpan = 2;
                HeaderRow.Cells.Add(HeaderCell);

                //Adding Period Column
                HeaderCell = new TableCell();
                HeaderCell.Text = "Consultation";
                HeaderCell.ColumnSpan = 3;
                HeaderCell.HorizontalAlign = HorizontalAlign.Center;
                HeaderRow.Cells.Add(HeaderCell);

                //Adding Audited By Column
                HeaderCell = new TableCell();
                HeaderCell.Text = "NU";
                HeaderCell.ColumnSpan = 3;
                HeaderCell.HorizontalAlign = HorizontalAlign.Center;
                HeaderRow.Cells.Add(HeaderCell);

                //Adding the Row at the 0th position (first row) in the Grid
                gridview.Controls[0].Controls.AddAt(0, HeaderRow);
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
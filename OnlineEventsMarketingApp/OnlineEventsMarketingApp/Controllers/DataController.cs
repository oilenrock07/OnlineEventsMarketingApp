using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Common.Enums;
using OnlineEventsMarketingApp.Common.Helpers;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Helpers;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Models.Data;

namespace OnlineEventsMarketingApp.Controllers
{
    public class DataController : Controller
    {
        private readonly IRepository<DataSheet> _dataSheetRepository;

        public DataController(IRepository<DataSheet> dataSheetRepository)
        {
            _dataSheetRepository = dataSheetRepository;
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

        [Authorize]
        [HttpPost]
        public ActionResult DataSheet(HttpPostedFileBase file)
        {
            return View();
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

        public void ExportToExcel(int month, int year)
        {

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);
            var datasheet = _dataSheetRepository.Find(x => x.Date >= startDate && x.Date < endDate);

            var fileName = String.Format("Nepro Report {0} {1}", startDate.ToString("MMMM"), year);

            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("DIS", typeof(int));
            dt.Columns.Add("TE", typeof(int));
            dt.Columns.Add("TM", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("InHouse", typeof(string));
            dt.Columns.Add("Rnd", typeof(string));
            dt.Columns.Add("Date", typeof(DateTime));

            dt.Columns.Add("New Users", typeof(int));
            dt.Columns.Add("Existing Users", typeof(int));
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("No Of Patients", typeof(int));
            dt.Columns.Add("Tag", typeof(int));

            foreach (var item in datasheet)
            {
                var row = dt.NewRow();
                row["ID"] = item.DataSheetId;
                row["DIS"] = item.DIS;
                row["TE"] = item.TE;
                row["TM"] = item.TM;
                row["Area"] = item.Area;
                row["InHouse"] = item.InHouse;
                row["Rnd"] = item.Rnd;
                row["Date"] = item.Date;

                row["New Users"] = item.NewUsers;
                row["Existing Users"] = item.ExistingUsers;
                row["Status"] = (DataStatus) item.Status;
                row["No Of Patients"] = item.NoOfPatients;
                row["Tag"] = item.TagId;
                dt.Rows.Add(row);
            }

            Export.ToExcel(Response, dt, fileName);
        }
    }
}
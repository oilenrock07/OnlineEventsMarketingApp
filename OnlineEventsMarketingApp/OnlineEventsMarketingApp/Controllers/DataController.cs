using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using OnlineEventsMarketingApp.Common.Enums;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Helpers;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Models.Data;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Controllers
{
    public class DataController : Controller
    {
        private readonly IRepository<DataSheet> _dataSheetRepository;
        private readonly ITagService _tagService;
        private readonly IDataSheetService _dataSheetService;

        public DataController(IDataSheetService dataSheetService, ITagService tagService, IRepository<DataSheet> dataSheetRepository)
        {
            _tagService = tagService;
            _dataSheetService = dataSheetService;
            _dataSheetRepository = dataSheetRepository;
        }

        [Authorize]
        public ActionResult DataSheet()
        {
            var now = DateTime.Now;
            var tags = _tagService.GetTags(now.Year, now.Month);

            var viewModel = new DataSheetViewModel
            {
                Year = now.Year,
                Month = now.Month,
                Tags = JsonConvert.SerializeObject(tags),
                Years = MonthYearHelper.GetYearList(),
                Months = MonthYearHelper.GetMonthList()
            };
            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetDataSheet(int month, int year)
        {
            var dateSheet = _dataSheetService.GetDataSheet(month, year);           
            return Json(dateSheet, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DataSheet(int month, int year, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.EndsWith(".csv"))
                {
                    using (var stream = file.InputStream)
                    {
                        using (var csvTable = new DataTable())
                        {
                            using (var reader = new CsvReader(new StreamReader(stream), true))
                            {
                                csvTable.Load(reader);
                                _dataSheetService.UploadDataSheet(month, year, csvTable);
                            }
                        }
                    }
                }
            }

            TempData["Message"] = "Datasheet has been successfully uploaded";
            var viewModel = new DataSheetViewModel
            {
                Year = year,
                Month = month,
                Years = MonthYearHelper.GetYearList(),
                Months = MonthYearHelper.GetMonthList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult PostDataSheetChanges(string data)
        {
            var datasheet = JsonConvert.DeserializeObject<IEnumerable<DataSheet>>(data);
            return Json("success");
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
                row["Status"] = (DataStatus)item.Status;
                row["No Of Patients"] = item.NoOfPatients;
                row["Tag"] = item.TagId;
                dt.Rows.Add(row);
            }

            Export.ToExcel(Response, dt, fileName);
        }
    }
}
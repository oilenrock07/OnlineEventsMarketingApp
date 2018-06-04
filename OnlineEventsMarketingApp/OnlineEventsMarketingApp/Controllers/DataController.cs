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
using OnlineEventsMarketingApp.Common.Extensions;
using OnlineEventsMarketingApp.Common.Helpers;
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
        private readonly IRepository<NewUserMTD> _newUserMTdRepository;
        private readonly IDataSheetService _dataSheetService;
        private readonly IUnitOfWork _unitOfWork;

        public DataController(IUnitOfWork unitOfWork, IDataSheetService dataSheetService, IRepository<NewUserMTD> newUserMTdRepository, IRepository<DataSheet> dataSheetRepository)
        {
            _unitOfWork = unitOfWork;
            _dataSheetService = dataSheetService;
            _newUserMTdRepository = newUserMTdRepository;
            _dataSheetRepository = dataSheetRepository;
        }

        [Authorize]
        public ActionResult DataSheet(int? month = null, int? year=null)
        {
            var now = DateTime.Now;
            var viewModel = new DataSheetViewModel
            {
                Year = year ?? now.Year,
                Month = month ?? now.Month,
                Years = MonthYearHelper.GetYearList(),
                Months = MonthYearHelper.GetMonthList()
            };
            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetDataSheet(int month, int year)
        {
            var dateSheet = _dataSheetService.GetDataSheet(month, year).OrderBy(x => x.Date);
            return Json(dateSheet, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DataSheet(int month, int year, HttpPostedFileBase file)
        {
            var viewModel = new DataSheetViewModel
            {
                Year = year,
                Month = month,
                Years = MonthYearHelper.GetYearList(),
                Months = MonthYearHelper.GetMonthList()
            };

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

                                var headers = new List<string> { "DISTID", "TERRID", "TM", "AREA", "In House", "RND", "Date", "New User", "Existing User", "STATUS", "# of PATIENTS" };
                                if (!csvTable.IsHeaderValid(headers))
                                {
                                    ModelState.AddModelError("", String.Format("Uploaded file does not contain all the required headers: {0}", String.Join(",", headers)));
                                    return View(viewModel);
                                }
                                    
                                _dataSheetService.UploadDataSheet(month, year, csvTable);
                            }
                        }
                    }
                }
            }

            //TempData["Message"] = "Datasheet has been successfully uploaded";
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult PostDataSheetChanges(string sheet)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var datasheet = JsonConvert.DeserializeObject<IEnumerable<DataSheet>>(sheet, settings);
            if (datasheet != null && datasheet.Any())
            {
                var idsToEdit = datasheet.Select(x => x.DataSheetId);
                var result = _dataSheetRepository.Find(x => idsToEdit.Contains(x.DataSheetId)).ToList();
                foreach (var data in result)
                {
                    var updatedSheet = datasheet.FirstOrDefault(x => x.DataSheetId == data.DataSheetId);
                    if (updatedSheet != null)
                    {
                        _dataSheetRepository.Update(data);
                        data.Area = updatedSheet.Area;
                        data.DIS = updatedSheet.DIS;
                        data.Date = updatedSheet.Date.AddDays(1); //bug on the UI that is submitting -1 days
                        data.ExistingUsers = updatedSheet.ExistingUsers;
                        data.InHouse = updatedSheet.InHouse;
                        data.NewUsers = updatedSheet.NewUsers;
                        data.NoOfPatients = updatedSheet.NoOfPatients;
                        data.Rnd = updatedSheet.Rnd;
                        data.Status = updatedSheet.Status.ToUpper();
                        data.TE = updatedSheet.TE;
                        data.TM = updatedSheet.TM;
                        data.TagId = updatedSheet.TagId;
                    }
                }

                _unitOfWork.Commit();
            }

            return Json("success");
        }

        [Authorize]
        public ActionResult NewUserMTDDataSheet(int? month = null, int ? year = null)
        {
            var now = DateTime.Now;
            var m = month ?? now.Month;
            var y = year ?? now.Year;

            var datasheet = _dataSheetService.GetMonthlyNewUserReport(y, m).ToList();
            var viewModel = new NewUserDataSheetViewModel
            {
                Year = y,
                Month = m,
                DataSheets = datasheet,
                Years = MonthYearHelper.GetYearList(),                
                Months = MonthYearHelper.GetMonthList()                
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult NewUserMTDDataSheet(int month, int year, HttpPostedFileBase file)
        {
            var viewModel = new NewUserDataSheetViewModel
            {
                Year = year,
                Month = month,
                Years = MonthYearHelper.GetYearList(),
                Months = MonthYearHelper.GetMonthList()
            };

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
                                var headers = new List<string> { "TM CODE", "BRAND", "Date", "FIRST USE"};
                                if (!csvTable.IsHeaderValid(headers))
                                {
                                    ModelState.AddModelError("", String.Format("Uploaded file does not contain all the required headers: {0}", String.Join(",", headers)));
                                    return View(viewModel);
                                }
                                _dataSheetService.UploadNewUserMTDDataSheet(year, csvTable);
                            }
                        }
                    }
                }
            }

            //TempData["Message"] = "New User MTD Datasheet has been successfully uploaded";
            return RedirectToAction("NewUserMTDDataSheet", new { month, year});
        }

        public void ExportToExcel(int month, int year)
        {

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);
            var datasheet = _dataSheetRepository.Find(x => x.Date >= startDate && x.Date < endDate).ToList();

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
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("No Of Patients", typeof(int));
            dt.Columns.Add("Tag", typeof(string));

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
                row["Date"] = item.Date.ToShortDateString();

                row["New Users"] = item.NewUsers;
                row["Existing Users"] = item.ExistingUsers;
                row["Status"] = item.Status.ToUpper();
                row["No Of Patients"] = item.NoOfPatients;
                row["Tag"] = item.Tag != null ? item.Tag.TagName : "";
                dt.Rows.Add(row);
            }

            Export.ToExcel(Response, dt, fileName);
        }        
    }
}
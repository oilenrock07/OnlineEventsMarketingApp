using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNet.Identity;
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
        private readonly IDataSheetService _dataSheetService;
        private readonly ITagService _tagService;
        private readonly IUnitOfWork _unitOfWork;

        public DataController(IUnitOfWork unitOfWork, IDataSheetService dataSheetService, ITagService tagService, IRepository<DataSheet> dataSheetRepository)
        {
            _unitOfWork = unitOfWork;
            _tagService = tagService;
            _dataSheetService = dataSheetService;
            _dataSheetRepository = dataSheetRepository;
        }

        [Authorize]
        public ActionResult DataSheet(int? month = null, int? year=null)
        {
            var now = DateTime.Now;
            var y = year ?? now.Year;
            var m = month ?? now.Month;
            var hasTags = _tagService.HasTags(y, m);

            var viewModel = new DataSheetViewModel
            {
                Year = y,
                Month = m,
                HasTags = hasTags,
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
                Months = MonthYearHelper.GetMonthList(),
                HasTags = true
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

                                if (!csvTable.IsHeaderValid(Common.Constants.Constants.DATASHEET_HEADERS))
                                {
                                    ModelState.AddModelError("", String.Format("Uploaded file does not contain all the required headers: {0}", String.Join(",", Common.Constants.Constants.DATASHEET_HEADERS)));
                                    return View(viewModel);
                                }

                                try
                                {
                                    _dataSheetService.UploadDataSheet(month, year, csvTable);
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", ex.Message);
                                }                                
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid file format. Please select only .csv file");
                }
            }

            //TempData["Message"] = "Datasheet has been successfully uploaded";
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult PostDataSheetChanges(int month, int year, string sheet)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var datasheet = JsonConvert.DeserializeObject<IEnumerable<DataSheet>>(sheet, settings);            
            if (datasheet != null && datasheet.Any())
            {
                var tags = _tagService.GetTags(datasheet.Where(x => x.Date != null).Select(x => x.Date.Value));
                var idsToEdit = datasheet.Select(x => x.DataSheetId);
                var result = _dataSheetRepository.Find(x => idsToEdit.Contains(x.DataSheetId)).ToList();
                foreach (var data in result)
                {
                    var updatedSheet = datasheet.FirstOrDefault(x => x.DataSheetId == data.DataSheetId);
                    if (updatedSheet != null)
                    {
                        var d = updatedSheet.Date;
                        var date = d != null? d.Value.AddDays(1) : default(DateTime?); //bug on the UI that is submitting -1 days
                        
                        _dataSheetRepository.Update(data);

                        if (data.Date.ToDateTime() != date.ToDateTime())
                        {
                            var tag = tags.FirstOrDefault(x => date >= x.StartDate && date <= x.EndDate);
                            data.TagId = tag?.TagId ?? 0;
                        }

                        if (date == null)
                            data.TempDate = new DateTime(year, month, 1);

                        data.Area = updatedSheet.Area;
                        data.DIS = updatedSheet.DIS;
                        data.Date = date;
                        data.ExistingUsers = updatedSheet.ExistingUsers;
                        data.InHouse = updatedSheet.InHouse;
                        data.NewUsers = updatedSheet.NewUsers;
                        data.NoOfPatients = updatedSheet.NoOfPatients;
                        data.Rnd = updatedSheet.Rnd;
                        data.Status = updatedSheet.Status.ToUpper();
                        data.TE = updatedSheet.TE;
                        data.TM = updatedSheet.TM;
                    }
                }

                _unitOfWork.Commit();
            }

            var dateSheet = _dataSheetService.GetDataSheet(month, year).OrderBy(x => x.Date);
            return Json(dateSheet, JsonRequestBehavior.AllowGet);
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
            var datasheet = _dataSheetRepository.Find(x => x.Date != null && x.Date >= startDate && x.Date < endDate).ToList();

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
                row["Date"] = item.Date.Value.ToShortDateString();

                row["New Users"] = item.NewUsers;
                row["Existing Users"] = item.ExistingUsers;
                row["Status"] = item.Status.ToUpper();
                row["No Of Patients"] = item.NoOfPatients;
                row["Tag"] = item.Tag != null ? item.Tag.TagName : "";
                dt.Rows.Add(row);
            }

            Export.ToExcel(Response, dt, fileName);
        }

        [HttpPost]
        public JsonResult ClearData(int month, int year)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            _dataSheetService.ClearDataSheet(start, end);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteData(int id)
        {
            var datasheet = _dataSheetRepository.FirstOrDefault(x => x.DataSheetId == id);
            if (datasheet == null)
                throw new Exception("Invalid ID");

            _dataSheetRepository.Delete(datasheet);
            _unitOfWork.Commit();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
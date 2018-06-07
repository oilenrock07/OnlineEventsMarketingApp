using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineEventsMarketingApp.Common.Extensions;
using OnlineEventsMarketingApp.Common.Helpers;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Entities.Contexts;
using OnlineEventsMarketingApp.Helpers;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Models.Settings;
using OnlineEventsMarketingApp.Services.Implementations;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IDataSheetService _dataSheetService;
        private readonly TagService _tagService;

        public SettingsController(IUnitOfWork unitOfWork, IRepository<Tag> tagRepository, IDataSheetService dataSheetService, TagService tagService)
        {
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
            _dataSheetService = dataSheetService;
            _tagService = tagService;
        }

        // GET: Settings
        [Authorize(Roles = "Admin")]
        public ActionResult Tags(int? month = null, int? year = null)
        {
            var now = DateTime.Now;
            var currentMonth = month ?? now.Month;
            var currentYear = year ?? now.Year;
            
            var tags = _tagService.GetTags(currentYear, currentMonth);

            var viewModel = new TagListViewModel
            {
                Year = currentYear,
                Month = currentMonth,
                Months = MonthYearHelper.GetMonthList(),
                Years = MonthYearHelper.GetYearList(),
                Tags = tags
            };

            return View(viewModel);
        }

        //public PartialViewResult TagsContent(int year, int month)
        //{
        //    var tags = _tagService.GetTags(year, month);
        //    var viewModel = new TagListViewModel
        //    {
        //        Tags = tags
        //    };
        //    return PartialView("_TagContent", viewModel);
        //}

        [Authorize(Roles = "Admin")]
        public ActionResult CreateTag()
        {
            var now = DateTime.Now;
            var currentMonth = now.Month;
            var currentYear = now.Year;

            var viewModel = new TagCreateViewModel
            {
                Month = currentMonth,
                Year = currentYear,
                Months = MonthYearHelper.GetMonthList(),
                Years = MonthYearHelper.GetYearList(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateTag(TagCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.StartDate > viewModel.EndDate)
                {
                    viewModel.Months = MonthYearHelper.GetMonthList();
                    viewModel.Years = MonthYearHelper.GetYearList();
                    ModelState.AddModelError("", "End date must not be greater than Start Date");
                    return View(viewModel);
                }

                //check for overlap
                var overlappedTags = _tagRepository.FirstOrDefault(x => !x.IsDeleted && x.Month == viewModel.Month && x.Year == viewModel.Year &&
                                                                  viewModel.StartDate <= x.EndDate && x.StartDate <= viewModel.EndDate);

                if (overlappedTags != null)
                {
                    viewModel.Months = MonthYearHelper.GetMonthList();
                    viewModel.Years = MonthYearHelper.GetYearList();
                    ModelState.AddModelError("", "Date must not overlap");
                    return View(viewModel);
                }

                var tag = viewModel.MapItem<Tag>();
                _tagRepository.Add(tag);
                _unitOfWork.Commit();

                return RedirectToAction("Tags", new { month = viewModel.Month, year = viewModel.Year});
            }

            viewModel.Months = MonthYearHelper.GetMonthList();
            viewModel.Years = MonthYearHelper.GetYearList();
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditTag(int id)
        {
            var tag = _tagRepository.GetById(id);
            if (tag == null)
                return RedirectToAction("Tags");
           
            var viewModel = tag.MapItem<TagCreateViewModel>();
            viewModel.Months = MonthYearHelper.GetMonthList();
            viewModel.Years = MonthYearHelper.GetYearList();
            viewModel.HasDataSheet = _dataSheetService.HasDataSheet(tag.Year, tag.Month);
            viewModel.Year = tag.Year;
            viewModel.Month = tag.Month;
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditTag(TagCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.StartDate > viewModel.EndDate)
                {
                    viewModel.Months = MonthYearHelper.GetMonthList();
                    viewModel.Years = MonthYearHelper.GetYearList();
                    ModelState.AddModelError("", "End date must not be greater than Start Date");
                    return View(viewModel);
                }

                var overlappedTags = _tagRepository.FirstOrDefault(x => !x.IsDeleted && x.Month == viewModel.Month && x.Year == viewModel.Year && x.TagId != viewModel.TagId &&
                                                                    viewModel.StartDate <= x.EndDate && x.StartDate <= viewModel.EndDate);

                if (overlappedTags != null)
                {
                    viewModel.Months = MonthYearHelper.GetMonthList();
                    viewModel.Years = MonthYearHelper.GetYearList();
                    ModelState.AddModelError("", "Date must not overlap");
                    return View(viewModel);
                }

                var tag = _tagRepository.GetById(viewModel.TagId);

                _tagRepository.Update(tag);
                tag.Month = viewModel.Month;
                tag.Year = viewModel.Year;
                tag.TagName = viewModel.TagName;
                tag.StartDate = viewModel.StartDate;
                tag.EndDate = viewModel.EndDate;

                _unitOfWork.Commit();
                return RedirectToAction("Tags", new { month = viewModel.Month, year = viewModel.Year});
            }

            viewModel.Months = MonthYearHelper.GetMonthList();
            viewModel.Years = MonthYearHelper.GetYearList();
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteTag(int id)
        {
            var tag = _tagRepository.GetById(id);
            if (tag == null)
                return RedirectToAction("Tags");

            _tagRepository.Update(tag);
            tag.IsDeleted = true;

            _unitOfWork.Commit();
            return RedirectToAction("Tags", new {month = tag.Month, year = tag.Year});
        }

        // GET: Settings/CreateTag
        //public ActionResult CreateTag(int month)
        //{
        //    var viewModel = new TagCreateViewModel
        //    {
        //        Month = month
        //    };

        //    return View(viewModel);
        //}

        //// POST: Settings/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public PartialViewResult CreateTag([Bind(Include = "TagId,TagName,Year,Month,StartDate,EndDate")] TagCreateViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var tag = viewModel.MapItem<Tag>();
        //        _tagRepository.Add(tag);
        //        _unitOfWork.Commit();
        //    }

        //    return TagsContent(viewModel.Year, viewModel.Month);
        //}



        //// GET: Settings/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Tag tag = db.Tags.Find(id);
        //    if (tag == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tag);
        //}

        //// POST: Settings/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "TagId,TagName,Month,StartDate,EndDate")] Tag tag)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tag).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(tag);
        //}

        //// GET: Settings/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Tag tag = db.Tags.Find(id);
        //    if (tag == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tag);
        //}

        //// POST: Settings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Tag tag = db.Tags.Find(id);
        //    db.Tags.Remove(tag);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

    }
}

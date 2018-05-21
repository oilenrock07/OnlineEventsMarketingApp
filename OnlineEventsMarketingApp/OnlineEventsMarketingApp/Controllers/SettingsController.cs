using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineEventsMarketingApp.Common.Extensions;
using OnlineEventsMarketingApp.Common.Helpers;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Entities.Contexts;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Models.Settings;
using OnlineEventsMarketingApp.Services.Implementations;

namespace OnlineEventsMarketingApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tag> _tagRepository;
        private readonly TagService _tagService;

        public SettingsController(IRepository<Tag> tagRepository, TagService tagService)
        {
            _tagRepository = tagRepository;
            _tagService = tagService;
        }

        // GET: Settings
        public ActionResult Tags()
        {
            var currentMonth = DateTime.Now.Month;
            var tags = _tagService.GetTagsByMonth(currentMonth);
            var viewModel = new TagListViewModel
            {
                Month = currentMonth,
                Months = GetMonthList(),
                Tags = tags
            };

            return View(viewModel);
        }

        public PartialViewResult TagsContent()
        {
            var tags = _tagService.GetTagsByMonth(currentMonth);
            var viewModel = new TagListViewModel
            {
                Tags = tags
            };
            return PartialView("_TagContent", );
        }

        // GET: Settings/CreateTag
        public ActionResult CreateTag(int month)
        {
            var viewModel = new TagCreateViewModel
            {
                Month = month
            };

            return View(viewModel);
        }

        //// POST: Settings/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult Create([Bind(Include = "TagId,TagName,Month,StartDate,EndDate")] TagCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tag = viewModel.MapItem<Tag>();
                _tagRepository.Add(tag);
                _unitOfWork.Commit();
            }

            return TagsContent();
        }

        

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

        private IEnumerable<SelectListItem> GetMonthList()
        {
            var months = MonthHelper.GetMonths().Select(x => new SelectListItem
            {
                Text = x.Item2,
                Value = x.Item1.ToString()
            });

            return months;
        } 
    }
}

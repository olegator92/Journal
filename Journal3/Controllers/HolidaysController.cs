﻿using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    
    public class HolidaysController : Controller
    {
        private ApplicationDbContext db = null;

        public HolidaysController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Holidays
        public ActionResult Index(int? year)
        {
            int currentYear = DateTime.Now.Year;
            if (year != null)
                currentYear = year.Value;
            List<Holiday> holidays = db.Holidays.Where(x => x.Date.Year == currentYear).ToList();

            ViewBag.DayOfWeek = Helper.DaysOfWeekHelper.GetDayName((int)DateTime.Now.DayOfWeek);
            ViewBag.SelectedYear = currentYear;
            ViewBag.PreviousYear = currentYear-1;
            ViewBag.NextYear = currentYear+1;

            return View(holidays);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var holiday = new Holiday();
            holiday.Date = DateTime.Now;
            ViewBag.DayOfWeek = Helper.DaysOfWeekHelper.GetDayName((int)DateTime.Now.DayOfWeek);
            return View(holiday);
        }

        // POST: Holidays/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Holiday holiday)
        {
            ViewBag.DayOfWeek = Helper.DaysOfWeekHelper.GetDayName((int)DateTime.Now.DayOfWeek);
            if (!db.Holidays.Any(x => EntityFunctions.TruncateTime(x.Date) == EntityFunctions.TruncateTime(holiday.Date)))
            {
                db.Holidays.Add(holiday);
                db.SaveChanges();
                return RedirectToAction("Index", new { year = holiday.Date.Year });
            }
            return RedirectToAction("Index");
        }

        // GET: Holidays/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var holiday = db.Holidays.Find(id);
            return View(holiday);
        }

        // POST: Holidays/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(Holiday holiday)
        {
            var dbHoliday = db.Holidays.Find(holiday.Id);
            if (ModelState.IsValid)
            {
                UpdateModel(dbHoliday);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(holiday);
        }

        // GET: Holidays/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var holiday = db.Holidays.Find(id);
            return View(holiday);
        }

        // POST: Holidays/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var holiday = db.Holidays.Find(id);
            try
            {
                db.Holidays.Remove(holiday);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно запись");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
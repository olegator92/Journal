using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    public class WorkSchedulesController : Controller
    {
        private ApplicationDbContext db = null;

        public WorkSchedulesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: WorkSchedules
        public ActionResult Index()
        {
            var workSchedules = db.WorkSchedules.ToList();
            return View(workSchedules);
        }

  
        // GET: WorkSchedules/Create
        public ActionResult Create()
        {
            var workSchedule = new WorkSchedule();
            return View(workSchedule);
        }

        // POST: WorkSchedules/Create
        [HttpPost]
        public ActionResult Create(WorkSchedule workSchedule)
        {
            db.WorkSchedules.Add(workSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: WorkSchedules/Edit/5
        public ActionResult Edit(int id)
        {
            var workSchedule = db.WorkSchedules.Find(id);
            return View(workSchedule);
        }

        // POST: WorkSchedules/Edit/5
        [HttpPost]
        public ActionResult Edit(WorkSchedule workSchedule)
        {
            var dbWorkSchedule = db.WorkSchedules.Find(workSchedule.Id);
            if (ModelState.IsValid)
            {
                UpdateModel(dbWorkSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workSchedule);
        }

        // GET: WorkSchedules/Delete/5
        public ActionResult Delete(int id)
        {
            var workSchedule = db.WorkSchedules.Find(id);
            return View(workSchedule);
        }

        // POST: WorkSchedules/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var WorkSchedule = db.WorkSchedules.Find(id);
            try
            {
                db.WorkSchedules.Remove(WorkSchedule);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно удалить график работы");
                //TempData["Message"] = "Невозможно удалить скидку.";
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}

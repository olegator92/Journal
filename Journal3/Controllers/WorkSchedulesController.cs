using Journal3.Enums;
using Journal3.Models;
using Journal3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    [Authorize(Roles = "Admin")]
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

            List<WorkScheduleViewModel> model = new List<WorkScheduleViewModel>();         
            var workSchedules = db.WorkSchedules.ToList();
            foreach (var workSchedule in workSchedules)
            {
                WorkScheduleViewModel schedule = new WorkScheduleViewModel();
                List<SpecialSchedule> specials = new List<SpecialSchedule>();
                schedule.WorkSchedule = workSchedule;
                var specialSchedules = db.SpecialSchedules.Where(x => x.WorkScheduleId == workSchedule.Id).ToList();
                if (specialSchedules.Any())
                {
                    schedule.SpecialSchedules = specialSchedules;
                }
                model.Add(schedule);
            }
            return View(model);
        }


        // GET: WorkSchedules/Create
        public ActionResult Create()
        {
            var model = new WorkScheduleViewModel();
            WorkSchedule workSchedule = new WorkSchedule();
            model.WorkSchedule = workSchedule;
            return View(model);
        }

        // POST: WorkSchedules/Create
        [HttpPost]
        public ActionResult Create(WorkScheduleViewModel model)
        {
            db.WorkSchedules.Add(model.WorkSchedule);
            db.SaveChanges();

            if (model.SpecialSchedules != null && model.SpecialSchedules.Any())
            {
                foreach (var special in model.SpecialSchedules)
                {
                    special.WorkSchedule = model.WorkSchedule;
                    db.SpecialSchedules.Add(special);
                }
                   
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: WorkSchedules/Edit/5
        public ActionResult Edit(int id)
        {
            
            WorkScheduleViewModel model = new WorkScheduleViewModel();
            var workSchedule = db.WorkSchedules.Find(id);
            model.WorkSchedule = workSchedule;

            if (workSchedule.IsSpecial)
            {
                var specialSchedules = db.SpecialSchedules.Where(x => x.WorkScheduleId == workSchedule.Id).ToList();
                model.SpecialSchedules = specialSchedules;
            }

            return View(model);
        }

        // POST: WorkSchedules/Edit/5
        [HttpPost]
        public ActionResult Edit(WorkScheduleViewModel model)
        {
            var dbWorkSchedule = db.WorkSchedules.Find(model.WorkSchedule.Id);
            dbWorkSchedule.Name = model.WorkSchedule.Name;
            dbWorkSchedule.StartWork = model.WorkSchedule.StartWork;
            dbWorkSchedule.EndWork = model.WorkSchedule.EndWork;
            dbWorkSchedule.IsSpecial = model.WorkSchedule.IsSpecial;

            var specialWorkSchedules = db.SpecialSchedules.Where(x => x.WorkScheduleId == model.WorkSchedule.Id).ToList();
            db.SpecialSchedules.RemoveRange(specialWorkSchedules);

            if (model.SpecialSchedules != null && model.SpecialSchedules.Any())
                db.SpecialSchedules.AddRange(model.SpecialSchedules);

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        // GET: WorkSchedules/Create
        /* public ActionResult Create()
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
         */
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

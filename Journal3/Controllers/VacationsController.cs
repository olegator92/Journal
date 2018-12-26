using Journal3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class VacationsController : Controller
    {
        private ApplicationDbContext db = null;
        private UserManager<ApplicationUser> userManager;
        // GET: Vacations
        public VacationsController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        public ActionResult Index(string userId = "")
        {
            List<Vacation> vacations = new List<Vacation>();

            if (User.IsInRole("Employee"))
            {
                userId = User.Identity.GetUserId();
            }

            if (userId != "")
            {
                vacations = db.Vacations.Where(x => x.UserId == userId)
                                            .OrderBy(x => x.Date)
                                            .ToList();
            }

            ViewBag.DayOfWeek = Helper.DaysOfWeekHelper.GetDayName((int)DateTime.Now.DayOfWeek);
            return View(vacations);
        }


        // POST: Vacations/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddVacations(List<Vacation> vacations)
        {
            foreach (Vacation vacation in vacations)
            {
                if (!db.Vacations.Any(x => x.Date == vacation.Date && x.UserId == vacation.UserId))
                {
                    db.Vacations.Add(vacation);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public void Delete(int vacationId)
        {
            var vacation = db.Vacations.Find(vacationId);
            try
            {
                db.Vacations.Remove(vacation);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно удалить запись");
            }
            return RedirectToAction("Index");
        }
    }
}

using Journal3.Models;
using Journal3.ViewModels;
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
            VacationsViewModel model = new VacationsViewModel();
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
            model.Dates = vacations.Select(x => x.Date).OrderBy(x => x).ToList();
            

            ViewBag.DayOfWeek = Helper.DaysOfWeekHelper.GetDayName((int)DateTime.Now.DayOfWeek);
            if (User.IsInRole("Admin"))
                ViewBag.User = new SelectList(db.UserInfoes.Where(x => x.Key != null).OrderBy(x => x.Name).ToList(), "UserId", "Name", userId);
            ViewBag.UserId = userId;
            return View(vacations);
        }

        // POST: Vacations/Edit/5
        public void AddVacations(DateTime date, string UserId)
        {
            if (!db.Vacations.Any(x => x.Date == date && x.UserId == UserId))
            {
                Vacation vacation = new Vacation();
                vacation.UserId = UserId;
                vacation.Date = date;
                db.Vacations.Add(vacation);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddVacations(DateTime startDate, DateTime endDate, string UserId)
        {
            if (startDate > endDate || (endDate - startDate).TotalDays > 30)
                return;

            while (startDate < endDate)
            {
                if (!db.Vacations.Any(x => x.Date == startDate && x.UserId == UserId))
                {
                    Vacation vacation = new Vacation();
                    vacation.UserId = UserId;
                    vacation.Date = startDate;
                    db.Vacations.Add(vacation);
                }
                startDate.AddDays(1);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
                throw new Exception("Невозможно удалить запись\n" + e.Message);
            }
        }
    }
}

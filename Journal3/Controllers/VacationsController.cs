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
        public void AddVacation(string dateStr, string UserId)
        {
            DateTime date = new DateTime();
            DateTime.TryParse(dateStr, out date);
            if (date != default(DateTime) && UserId != "")
            {
                if (!db.Vacations.Any(x => x.Date == date && x.UserId == UserId))
                {
                    var specials = db.SpecialSchedules.Where(x => x.WorkScheduleId == db.UserInfoes.FirstOrDefault(y => y.UserId == UserId).WorkScheduleId).ToList();
                    int dayOfWeek = (int)date.DayOfWeek;
                    if (specials.Any(x => x.DayOfWeek == dayOfWeek) || dayOfWeek != 0 && dayOfWeek != 6)
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
            }
        }

        public void AddVacations(string startDateStr, string endDateStr, string UserId)
        {
            if (startDateStr != "" && endDateStr != "" && UserId != "")
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                DateTime.TryParse(startDateStr, out startDate);
                DateTime.TryParse(endDateStr, out endDate);

                if (startDate > endDate || (endDate - startDate).TotalDays > 30)
                    return;

                var specials = db.SpecialSchedules.Where(x => x.WorkScheduleId == db.UserInfoes.FirstOrDefault(y => y.UserId == UserId).WorkScheduleId).ToList();
                while (startDate < endDate)
                {
                    if (!db.Vacations.Any(x => x.Date == startDate && x.UserId == UserId))
                    {
                        int dayOfWeek = (int)startDate.DayOfWeek;
                        if (specials.Any(x => x.DayOfWeek == dayOfWeek) || dayOfWeek != 0 && dayOfWeek != 6)
                        {
                            Vacation vacation = new Vacation();
                            vacation.UserId = UserId;
                            vacation.Date = startDate;
                            db.Vacations.Add(vacation);
                        }    
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
        }

        public void Delete(int id)
        {
            var vacation = db.Vacations.Find(id);
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

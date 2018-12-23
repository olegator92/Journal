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
            /*if (userId != "")
            {
                vacations = db.Vacations.Where(x => x.UserId == userId)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();
            }
            else
            {
                records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate)
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();
            }*/
            return View();
        }

        // GET: Vacations/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vacations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vacations/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vacations/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Vacations/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vacations/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Vacations/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

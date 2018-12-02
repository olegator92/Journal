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
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private ApplicationDbContext db = null;
        private UserManager<ApplicationUser> userManager;

        public RolesController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        // GET: Roles
        public ActionResult Index()
        {
            
            var Roles = db.Roles.ToList();
            return View(Roles);
        }


        // GET: Roles/Create
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            if (!context.Roles.Any(x => x.Name == Role.Name))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                manager.Create(Role);
                return RedirectToAction("Index");
            }
            return View(Role);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(string id)
        {
            var Role = db.Roles.Find(id);
            return View(Role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult Edit(IdentityRole Role)
        {
            var dbRole = db.Roles.Find(Role.Id);
            if (ModelState.IsValid)
            {
                UpdateModel(dbRole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Role);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {
            var Role = db.Roles.Find(id);
            return View(Role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            var Role = db.Roles.Find(id);
            try
            {
                db.Roles.Remove(Role);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно удалить роль");
                //TempData["Message"] = "Невозможно удалить скидку.";
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                var s = userManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
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

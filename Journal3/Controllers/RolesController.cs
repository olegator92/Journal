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
            /*if (User.Identity.IsAuthenticated)
            {

                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }*/
            
            var Roles = db.Roles.ToList();
            return View(Roles);
        }

        // GET: Roles/Details/5
        /*public ActionResult Details(int id)
        {
            return View();
        }*/

        // GET: Roles/Create
        public ActionResult Create()
        {
            /*if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }*/

            var Role = new IdentityRole();
            return View(Role);
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            /*if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }*/

            db.Roles.Add(Role);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            if (ModelState.IsValid)
            {
                UpdateModel(Role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Role);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Roles/Delete/5
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
    }
}

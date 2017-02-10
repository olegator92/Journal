using Journal3.Models;
using Journal3.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using System.Net;

namespace Journal3.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = null;
        private ApplicationUserManager _userManager;
        public UsersController()
        {
            db = new ApplicationDbContext();
        }

        public UsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            List<UserViewModel> usersModel = new List<UserViewModel>();
            var roleAdminId = db.Roles.FirstOrDefault(x => x.Name == "Admin").Id;
            var users = db.Users.Where(x => x.Roles.Any(i => i.RoleId != roleAdminId)).ToList();
            if (users.Any())
            {
                foreach (var user in users)
                {
                    UserViewModel model = new UserViewModel();
                    model.UserId = user.Id;
                    model.Email = user.UserName;
                    model.Role = UserManager.GetRoles(user.Id).FirstOrDefault();
                    var userInfo = db.UserInfoes.Where(x => x.User.Id == user.Id).Include(x => x.WorkSchedule).FirstOrDefault();
                    if (userInfo != null)
                    {
                        model.Name = userInfo.Name;
                        model.Key = userInfo.Key;
                        if(userInfo.WorkSchedule != null)
                            model.WorkSchedule = userInfo.WorkSchedule;
                    }
                    usersModel.Add(model);
                }

            }
            return View(usersModel.OrderBy(x => x.Name));
        }


        // GET: Users/Create
        public ActionResult Create()
        {
            RegisterViewModel model = new RegisterViewModel();
            ViewBag.UserRoles = new SelectList(db.Roles, "Name", "Name");
            ViewBag.WorkSchedules = new SelectList(db.WorkSchedules, "Id", "Name");

            return View(model);
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = UserManager.Create(user, model.Password);
            if (result.Succeeded)
            { 
                UserManager.AddToRole(user.Id, model.UserRoles);

                UserInfo userInfo = new UserInfo();
                userInfo.User = db.Users.Find(user.Id);
                userInfo.Key = model.Key;
                userInfo.Name = model.UserName;
                userInfo.WorkSchedule = db.WorkSchedules.FirstOrDefault(x => x.Id == model.WorkSchedules);
                db.UserInfoes.Add(userInfo);
                db.SaveChanges();

                return RedirectToAction("Index");
                
            }
            
            ViewBag.UserRoles = new SelectList(db.Roles, "Name", "Name");
            ViewBag.WorkSchedules = new SelectList(db.WorkSchedules, "Id", "Name");
            return View(model);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);
            UserViewModel model = new UserViewModel();
            model.Email = user.UserName;
            var userInfo = db.UserInfoes.Where(x => x.User.Id == user.Id).Include(x => x.WorkSchedule).FirstOrDefault();
            if (userInfo != null)
            {
                model.Name = userInfo.Name;
                model.Key = userInfo.Key;
            }

            ViewBag.UserRoles = new SelectList(db.Roles, "Name", "Name", UserManager.GetRoles(user.Id).FirstOrDefault());
            ViewBag.WorkSchedules = new SelectList(db.WorkSchedules, "Id", "Name", userInfo.WorkSchedule);
            return View();
        }

        // POST: Users/Edit/5
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

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
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

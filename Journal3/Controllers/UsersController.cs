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

namespace Journal3.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = null;

        public UsersController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Users
        public ActionResult Index()
        {
            List<UserViewModel> usersModel = new List<UserViewModel>();
            var roleAdminId = db.Roles.FirstOrDefault(x => x.Name == "Admin").Id;
            var users = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleAdminId)).ToList();
            if (users.Any())
            {
                foreach (var user in users)
                {
                    UserViewModel model = new UserViewModel();
                    model.UserId = user.Id;
                    model.Email = user.UserName;
     
                    var userInfo = db.UserInfoes.FirstOrDefault(x => x.User.Id == user.Id);
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
            return View(usersModel);
        }

        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
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

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
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

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
    //[Authorize(Roles = "Admin")]
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
        public ActionResult Index(string Role = "Employee")
        {
            List<UserViewModel> usersModel = new List<UserViewModel>();
            var roleId = db.Roles.FirstOrDefault(x => x.Name == Role).Id;
            var users = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId) || !x.Roles.Any()).ToList();
            if (users.Any())
            {
                foreach (var user in users)
                {
                    UserViewModel model = new UserViewModel();
                    model.UserId = user.Id;
                    model.Email = user.UserName;
                    model.Role = UserManager.GetRoles(user.Id).FirstOrDefault();
                    /*var userInfo = db.UserInfoes.Where(x => x.User.Id == user.Id).Include(x => x.WorkSchedule).FirstOrDefault();
                    if (userInfo != null)
                    {
                        model.Name = userInfo.Name;
                        model.Key = userInfo.Key;
                        //model.WorkSchedule = userInfo.WorkSchedule;
                    }*/
                    usersModel.Add(model);
                }

            }
            ViewBag.Roles = new SelectList(db.Roles.ToList(), "Name", "Name", Role);

            return View(usersModel.OrderBy(x => x.Name));
        }


        // GET: Users/Create
        public ActionResult Create()
        {
            RegisterViewModel model = new RegisterViewModel();
            ViewBag.UserRoles = new SelectList(db.Roles.ToList(), "Name", "Name");
            //ViewBag.WorkSchedules = new SelectList(db.WorkSchedules.ToList(), "Id", "Name");

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
                //userInfo.UserId = user.Id;
                //userInfo.User = db.Users.Find(user.Id);
                userInfo.Key = model.Key;
                userInfo.Name = model.UserName;
                //userInfo.WorkSchedule = db.WorkSchedules.FirstOrDefault(x => x.Id == model.WorkSchedules);
                //db.UserInfoes.Add(userInfo);
                db.SaveChanges();

                return RedirectToAction("Index", new { Role = model.UserRoles});
                
            }
            
            ViewBag.UserRoles = new SelectList(db.Roles, "Name", "Name");
            //ViewBag.WorkSchedules = new SelectList(db.WorkSchedules, "Id", "Name");
            return View(model);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);
            RegisterViewModel model = new RegisterViewModel();
            model.Email = user.UserName;
            /*var userInfo = db.UserInfoes.Where(x => x.User.Id == user.Id).Include(x => x.WorkSchedule).FirstOrDefault();
            if (userInfo != null)
            {
                model.UserName = userInfo.Name;
                model.Key = userInfo.Key;
                if(userInfo.WorkSchedule != null)
                    model.WorkSchedules = userInfo.WorkSchedule.Id ;
            }*/
            
            ViewBag.UserId = id;
            ViewBag.UserRole = new SelectList(db.Roles.ToList(), "Name", "Name", UserManager.GetRoles(user.Id).FirstOrDefault());
            /*if(userInfo != null && userInfo.WorkSchedule != null)
                ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name", userInfo.WorkSchedule.Id);
            else
                ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name");*/
            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, RegisterViewModel model)
        {
            var user = db.Users.Find(id);
            /*var dbUserInfo = db.UserInfoes.Where(x => x.User.Id == user.Id).Include(x => x.WorkSchedule).FirstOrDefault();

            if (dbUserInfo != null)
            {
                if (dbUserInfo.Name != model.UserName)
                    dbUserInfo.Name = model.UserName;
                if (dbUserInfo.WorkSchedule == null || (dbUserInfo.WorkSchedule != null && dbUserInfo.WorkSchedule.Id != model.WorkSchedules))
                    dbUserInfo.WorkSchedule = db.WorkSchedules.FirstOrDefault(x => x.Id == model.WorkSchedules);
                if (dbUserInfo.Key != model.Key)
                    dbUserInfo.Key = model.Key;
            }
            else
            {
                UserInfo newUserInfo = new UserInfo
                {
                    User = user,
                    UserId = user.Id,
                    Name = model.UserName,
                    //WorkSchedule = db.WorkSchedules.FirstOrDefault(x => x.Id == model.WorkSchedules),
                    Key = model.Key
                };
                db.UserInfoes.Add(newUserInfo);
                db.SaveChanges();
            }*/
            
            var role = UserManager.GetRoles(user.Id).FirstOrDefault();
            if (role != model.UserRoles)
            {
                if(!String.IsNullOrEmpty(role))
                    UserManager.RemoveFromRole(user.Id, role);
                UserManager.AddToRole(user.Id, model.UserRoles);
            }

            try
            {
                db.SaveChanges();
                return RedirectToAction("Index", new { Role = model.UserRoles });
            }
            catch
            {
                ViewBag.UserId = id;
                /*if (dbUserInfo.WorkSchedule != null)
                    ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name", dbUserInfo.WorkSchedule.Id);
                else
                    ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name");*/
                return View(model);
            }

        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            var user = db.Users.Find(id);
            RegisterViewModel model = new RegisterViewModel();
            model.Email = user.UserName;
            /*var userInfo = db.UserInfoes.Where(x => x.User.Id == user.Id).Include(x => x.WorkSchedule).FirstOrDefault();
            if (userInfo != null)
            {
                model.UserName = userInfo.Name;
                model.Key = userInfo.Key;
                if (userInfo.WorkSchedule != null)
                    ViewBag.WorkSchedule = db.WorkSchedules.Find(userInfo.WorkSchedule.Id).Name;
            }
            */
            ViewBag.UserId = id;
            ViewBag.UserRole = UserManager.GetRoles(user.Id).FirstOrDefault();

            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, RegisterViewModel model)
        {
            var user = db.Users.Find(id);
            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно удалить пользователя. " + e.Message);
            }

            return RedirectToAction("Index", new { Role = model.UserRoles });
        }
    }
}

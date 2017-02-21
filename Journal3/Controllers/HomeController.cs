using Journal3.Enums;
using Journal3.Models;
using Journal3.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = null;
        private UserManager<ApplicationUser> userManager;

        public HomeController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        public ActionResult Index(DateTime? selectedDate)
        {
            if (selectedDate == null)
                selectedDate = DateTime.Now.Date;
            var records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate)
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();

            List<RecordViewModel> model = new List<RecordViewModel>();
            if (records.Any())
            {
                foreach (var item in records)
                {
                    RecordViewModel record = new RecordViewModel();
                    record.Id = item.Id;
                    record.DateCreated = item.DateCreated;
                    record.DateRecord = item.DateRecord;
                    record.TimeRecord = item.TimeRecord;
                    record.Status = item.Status;
                    switch(item.Status)
                    {
                        case (int)Statuses.Come:
                            record.StatusName = "Пришел";
                            break;
                        case (int)Statuses.Gone:
                            record.StatusName = "Ушел";
                            break;
                        
                    }
                    record.Remark = item.Remark;
                    switch (item.Remark)
                    {
                        case (int)Remarks.ComeGone:
                            record.RemarkName = "Пришел/Ушел";
                            break;
                        case (int)Remarks.OutForWork:
                            record.RemarkName = "Выезд по работе";
                            break;
                        case (int)Remarks.ByPermission:
                            record.RemarkName = "Отпросился";
                            break;
                        case (int)Remarks.DebtWork:
                            record.RemarkName = "Отработка";
                            break;
                    }
                    record.Comment = item.Comment;
                    record.IsConfirmed = item.IsConfirmed;
                    record.IsForgiven = item.IsForgiven;
                    record.IsSystem = item.IsSystem;
                    record.User = item.User;
                    record.WorkSchedule = item.WorkSchedule;

                    model.Add(record);
                }

            }
           
            ViewBag.SelectedDate = selectedDate;
            return View(model);
        }


        public ActionResult Create(DateTime? selectedTime)
        {
            Record record = new Record();
            record.DateRecord = selectedTime ?? DateTime.Now;
            record.TimeRecord = DateTime.Now.TimeOfDay;

            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo);
                ViewBag.UserId = new SelectList(userInfoes, "UserId", "Name");
            }
            return View(record);
        }

        [HttpPost]
        public ActionResult Create(string UserId, Record model)
        {
            if(UserId == null)
               UserId = User.Identity.GetUserId();

            var dbUser = db.Users.Include(x => x.UserInfo).Include(x => x.UserInfo.WorkSchedule).FirstOrDefault(x => x.Id == UserId);
            if (dbUser != null)
            {
                string key = "";
                if (dbUser.UserInfo != null)
                    key = dbUser.UserInfo.Key;
                if (key != "")
                {
                    model.DateCreated = DateTime.Now;
                    model.User = dbUser;
                    model.IsConfirmed = false;
                    model.IsForgiven = false;
                    model.IsSystem = false;
                    model.WorkSchedule = dbUser.UserInfo.WorkSchedule;
                    db.Records.Add(model);
                    db.SaveChanges();
                }
            }
            
            return RedirectToAction("Index", new { selectedDate = model.DateRecord});
        }

        public ActionResult Edit(int id)
        {
            var record = db.Records.Include(x => x.WorkSchedule).Include(x => x.User).FirstOrDefault(x => x.Id == id);

            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo);
                ViewBag.UserId = new SelectList(userInfoes, "UserId", "Name", record.User.Id);
            }
            ViewBag.SelectedDate = record.DateRecord;
            return View(record);
        }

        [HttpPost]
        public ActionResult Edit(Record record)
        {
            var dbRecord = db.Records.Find(record.Id);
            if (ModelState.IsValid)
            {
                UpdateModel(dbRecord);
                db.SaveChanges();
                return RedirectToAction("Index", new { selectedDate = record.DateRecord});
            }
            ViewBag.SelectedDate = record.DateRecord;
            return View(record);
        }

        public ActionResult Delete(int id)
        {
            var record = db.Records.Include(x => x.User.UserInfo).Include(x => x.WorkSchedule).FirstOrDefault(x => x.Id == id);
            ViewBag.SelectedDate = record.DateRecord;
            return View(record);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var record = db.Records.Find(id);
            try
            {
                db.Records.Remove(record);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно удалить запись!");
                //TempData["Message"] = "Невозможно удалить скидку.";
                //return RedirectToAction("Index");
            }
            return RedirectToAction("Index", new { selectedDate = record.DateRecord});
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
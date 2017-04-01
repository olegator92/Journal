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

            UpdateDataFromFile(selectedDate);

            var records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate)
                                            .Include(x => x.WorkSchedule)
                                            //.Include(x => x.User.UserInfo)
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
                    record.DebtWorkDate = item.DebtWorkDate;
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
                    record.IsLate = item.IsLate;
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

        public ActionResult DayStats(DateTime? selectedDate)
        {
            if (selectedDate == null)
                selectedDate = DateTime.Now.Date;
            List<JournalViewModel> model = new List<JournalViewModel>();
            var records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate && x.IsConfirmed == true)
                                            .Include(x => x.WorkSchedule)
                                            //.Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();

            if (records.Any())
            {
                foreach (var user in records.Select(x => x.User).Distinct())
                {
                    JournalViewModel journalRow = new JournalViewModel();
                    //journalRow.EmployeeName = user.UserInfo.Name;
                    var first = records.FirstOrDefault(x => x.User == user && x.Status == (int)Statuses.Come);
                    if(first != null)
                        journalRow.ComeTime = first.TimeRecord;
                    var last = records.LastOrDefault(x => x.User == user && x.Status == (int)Statuses.Gone);
                    if(last != null)
                        journalRow.GoneTime = last.TimeRecord;

                    model.Add(journalRow);
                }

            }
            ViewBag.SelectedDate = selectedDate;
            return View(model.OrderBy(x => x.EmployeeName));
        }

        public ActionResult Create(DateTime? selectedDate)
        {
            Record record = new Record();
            record.DateRecord = selectedDate ?? DateTime.Now;
            record.TimeRecord = DateTime.Now.TimeOfDay;
            
            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                /*var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo);
                ViewBag.UserId = new SelectList(userInfoes, "UserId", "Name");*/
            }
            ViewBag.SelectedDate = selectedDate;
            return View(record);
        }

        [HttpPost]
        public ActionResult Create(string UserId, Record model)
        {
            if(UserId == null)
               UserId = User.Identity.GetUserId();

            var dbUser = db.Users/*.Include(x => x.UserInfo).Include(x => x.UserInfo.WorkSchedule)*/.FirstOrDefault(x => x.Id == UserId);
            if (dbUser != null)
            {
                string key = "";
                /*if (dbUser.UserInfo != null)
                    key = dbUser.UserInfo.Key*/
                if (key != "")
                {
                    if (model.Remark == (int)Remarks.DebtWork && model.DebtWorkDate == null)
                        return View(UserId, model);

                    if (User.IsInRole("Admin"))
                        model.IsConfirmed = true;
                    else
                        model.IsConfirmed = false;

                    model.DateCreated = DateTime.Now;
                    model.User = dbUser;
                    
                    model.IsForgiven = false;
                    model.IsSystem = false;
                    //model.WorkSchedule = dbUser.UserInfo.WorkSchedule;
                    if (model.Status == (int)Statuses.Come)
                    {
                        /*if ((model.TimeRecord - dbUser.UserInfo.WorkSchedule.StartWork).TotalMinutes > 5)
                            model.IsLate = true;
                        else
                            model.IsLate = false;*/
                    }
                    else if (model.Status == (int)Statuses.Gone)
                    {
                        /*if ((dbUser.UserInfo.WorkSchedule.EndWork - model.TimeRecord).TotalMinutes > 5)
                            model.IsLate = true;
                        else
                            model.IsLate = false;*/
                    }
                    db.Records.Add(model);
                    db.SaveChanges();
                }
            }
            ViewBag.SelectedDate = model.DateRecord;
            return RedirectToAction("Index", new { selectedDate = model.DateRecord});
        }

        public ActionResult Edit(int id)
        {
            var record = db.Records.Include(x => x.WorkSchedule).Include(x => x.User).FirstOrDefault(x => x.Id == id);

            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                /*var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo).ToList();
                var userInfoesId = db.UserInfoes.FirstOrDefault(x => x.UserId == record.User.Id);
                ViewBag.UserId = new SelectList(userInfoes, "UserId", "Name", userInfoesId.Id);*/
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
            if (!User.IsInRole("Admin"))
                record.IsConfirmed = false;
            else
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                /*var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo).OrderBy(x => x.Name);
                ViewBag.UserId = new SelectList(userInfoes, "UserId", "Name", record.User.Id);*/
            }
            ViewBag.SelectedDate = record.DateRecord;
            return View(record);
        }

        public ActionResult Delete(int id)
        {
            var record = db.Records/*.Include(x => x.User.UserInfo)*/.Include(x => x.WorkSchedule).FirstOrDefault(x => x.Id == id);
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


        public void ConfirmRecord(int recordId)
        {
            var record = db.Records.Find(recordId);
            if (record != null)
            {
                if (record.IsConfirmed == false)
                    record.IsConfirmed = true;
                else
                    record.IsConfirmed = false;
                db.SaveChanges();
            }
        }

        public void ConfirmAll(string date)
        {
            DateTime selectedDate = Convert.ToDateTime(date);
            var records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate.Date && x.IsSystem == false && x.IsConfirmed == false).ToList();
            foreach (var record in records)
            {
                record.IsConfirmed = true;
                db.SaveChanges();
            }
        }

        public void ForgiveRecord(int recordId)
        {
            var record = db.Records.Find(recordId);
            if (record != null)
            {
                if (record.IsForgiven == false)
                    record.IsForgiven = true;
                else
                    record.IsForgiven = false;
                db.SaveChanges();
            }
        }

        public void ForgiveAll(string date)
        {
            DateTime selectedDate = Convert.ToDateTime(date);
            var records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate.Date && x.IsSystem == false && x.IsForgiven == false && x.IsLate == true).ToList();
            foreach (var record in records)
            {
                record.IsForgiven = true;
                db.SaveChanges();
            }
        }

        public void UpdateDataFromFile(DateTime? selectedDate)
        {
            var setting = db.Settings.FirstOrDefault();
            if (setting != null)
            {
                string path = setting.FilePath;
                string fileText = System.IO.File.ReadAllText(path);
                if (fileText != "")
                {

                    int index = fileText.IndexOf(selectedDate.Value.ToString("dd.MM.yyyy"), 0);
                    if (index >= 0)
                    {
                        string todayFileText = fileText.Remove(0, index).Trim();
                        List<string> fileStrings = todayFileText.Split('\n').ToList();
                        if (fileStrings.Any())
                        {
                            List<FileRecordViewModel> fileRecords = GetFileRecords(fileStrings, selectedDate);

                            if (fileRecords.Any())
                            {
                                var dbRecords = db.Records.Where(x => x.IsSystem == true
                                                                    && DbFunctions.TruncateTime(x.DateRecord) == selectedDate)
                                                                    .OrderBy(x => x.TimeRecord).ToList();

                                if (fileRecords.Count() > dbRecords.Count())
                                {
                                    List<FileRecordViewModel> newRecords = GetNewRecords(dbRecords, fileRecords);
        
                                    if (newRecords.Any())
                                    {
                                        foreach (var newRecord in newRecords)
                                        {
                                            var user = GetUserByKey(newRecord.Key);
                                            if (user != null)
                                            {
                                                Record record = new Record();
                                                record.DateCreated = newRecord.Date;
                                                record.DateRecord = newRecord.Date;
                                                record.TimeRecord = newRecord.Time;
                                                record.IsConfirmed = true;
                                                record.IsSystem = true;
                                                record.IsForgiven = false;
                                                record.User = user;
                                                //record.WorkSchedule = user.UserInfo.WorkSchedule;

                                                record.Remark = (int)Remarks.ComeGone;
                                                if (!db.Records.Where(x => x.DateRecord == selectedDate).Any(x => x.User.Id == user.Id))
                                                    record.Status = (int)Statuses.Come;
                                                else
                                                    record.Status = (int)Statuses.Gone;

                                                if (record.Status == (int)Statuses.Come)
                                                {
                                                    /*if ((newRecord.Time - user.UserInfo.WorkSchedule.StartWork).TotalMinutes > 5)
                                                        record.IsLate = true;
                                                    else
                                                        record.IsLate = false;*/
                                                }
                                                else if(record.Status == (int)Statuses.Gone)
                                                {
                                                    /*if ((user.UserInfo.WorkSchedule.EndWork - newRecord.Time).TotalMinutes > 5)
                                                        record.IsLate = true;
                                                    else
                                                        record.IsLate = false;*/
                                                }

                                                db.Records.Add(record);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
                
            }
        }

        public ApplicationUser GetUserByKey(string key)
        {
            var user = db.Users/*.Include(x => x.UserInfo).Include(x => x.UserInfo.WorkSchedule)*/.FirstOrDefault(/*x => x.UserInfo.Key == key*/);
            return user;
        }

        List<FileRecordViewModel> GetFileRecords(List<string> fileStrings, DateTime? selectedDate)
        {
            List<FileRecordViewModel> fileRecords = new List<FileRecordViewModel>();
            int j = 0;
            foreach (string str in fileStrings)
            {
                string[] rec = str.Split('\t');
                if (Convert.ToDateTime(rec[0]) > selectedDate)
                    break;

                FileRecordViewModel fileRecord = new FileRecordViewModel
                {
                    Date = Convert.ToDateTime(rec[0]),
                    Time = TimeSpan.Parse(rec[1]),
                    Key = rec[2].Replace("\r", "")
                };

                if (fileRecords.Count() > 1)
                {
                    if (fileRecords[fileRecords.Count - 1].Key == fileRecord.Key && (fileRecords[fileRecords.Count - 1].Time - fileRecord.Time).TotalMinutes < 30)
                        continue;
                }
                fileRecords.Add(fileRecord);
                j++;
            }

            return fileRecords;
        }

        List<FileRecordViewModel> GetNewRecords(List<Record> dbRecords, List<FileRecordViewModel> fileRecords)
        {
            List<FileRecordViewModel> newRecords = new List<FileRecordViewModel>();
            if (!dbRecords.Any())
                newRecords = fileRecords.ToList();
            else
                newRecords = fileRecords.Where(x => !dbRecords.Any(i => i.TimeRecord == x.Time)).ToList();

            return newRecords;
        }
    }
}
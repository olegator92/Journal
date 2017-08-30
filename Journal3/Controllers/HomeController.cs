using Journal3.Enums;
using Journal3.Models;
using Journal3.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = null;
        private UserManager<ApplicationUser> userManager;

        public HomeController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        public ActionResult Index(DateTime? selectedDate, string userId = "", bool notConfirmed = false, string sortOrder = "time")
        {
            if (selectedDate == null)
                selectedDate = DateTime.Now.Date;

            bool fileStatus = UpdateDataFromFile(selectedDate);
            if (!fileStatus)
                ViewBag.FileStatus = "Проблемы с чтением файла!";
            else
                ViewBag.FileStatus = "";

            List<Record> records = new List<Record>();

            if (User.IsInRole("Employee"))
            {
                userId = User.Identity.GetUserId();
            }
            if (userId != "")
            {
                records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate && x.UserId == userId)
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
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
            }

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
                    switch (item.Status)
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
                        case (int)Remarks.OverWork:
                            record.RemarkName = "Переработка";
                            break;
                    }
                    record.Comment = item.Comment;
                    record.WithoutTimebreak = item.WithoutTimebreak;
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
            ViewBag.PreviousDate = selectedDate.Value.AddDays(-1);
            ViewBag.NextDate = selectedDate.Value.AddDays(1);
            if (User.IsInRole("Admin"))
                ViewBag.User = new SelectList(db.UserInfoes.Where(x => x.Key != null).OrderBy(x => x.Name).ToList(), "UserId", "Name", userId);
            ViewBag.UserId = userId;

            if(notConfirmed)
                model = model.Where(x => x.IsConfirmed == false).ToList();

            switch (sortOrder)
            {
                case "time":
                    model = model.OrderBy(s => s.TimeRecord).ToList();
                    break;
                case "name":
                    model = model.OrderBy(s => s.User.UserInfo.Name).ToList();
                    break;
                case "status":
                    model = model.OrderBy(s => s.StatusName).ToList();
                    break;
                case "remark":
                    model = model.OrderBy(s => s.RemarkName).ToList();
                    break;
                case "comment":
                    model = model.OrderBy(s => s.Comment).ToList();
                    break;
                case "withoutTimebreak":
                    model = model.OrderBy(s => s.WithoutTimebreak).ToList();
                    break;
                case "workschedule":
                    model = model.OrderBy(s => s.WorkSchedule.Name).ToList();
                    break;
                case "dateCreated":
                    model = model.OrderBy(s => s.DateCreated).ToList();
                    break;
                case "isSystem":
                    model = model.OrderBy(s => s.IsSystem).ToList();
                    break;
                default:
                    model = model.OrderBy(s => s.TimeRecord).ToList();
                    break;
            }

            return View(model);
        }

        public ActionResult DayStats(DateTime? selectedDate, string sortOrder = "name")
        {
            if (selectedDate == null)
                selectedDate = DateTime.Now.Date;

            bool fileStatus = UpdateDataFromFile(selectedDate);
            if (!fileStatus)
                ViewBag.FileStatus = "Проблемы с чтением файла!";
            else
                ViewBag.FileStatus = "";
            StatsViewModel model = new StatsViewModel();
            List<JournalViewModel> dayStats = new List<JournalViewModel>();
            var records = new List<Record>();

            if(db.Records.Any(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate && x.IsConfirmed == false))
                    model.IsNotConfirmeds = true;

            if (User.IsInRole("Employee"))
            {
                string userId = User.Identity.GetUserId();
                records = db.Records.Where(x => (DbFunctions.TruncateTime(x.DateRecord) == selectedDate || DbFunctions.TruncateTime(x.DebtWorkDate) == selectedDate) && x.IsConfirmed == true && x.UserId == userId)
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();
            }
            else
            {
                records = db.Records.Where(x => (DbFunctions.TruncateTime(x.DateRecord) == selectedDate || DbFunctions.TruncateTime(x.DebtWorkDate) == selectedDate) && x.IsConfirmed == true)
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();
            }

            if (records.Any())
            {
                foreach (var user in records.Select(x => x.User).Distinct().ToList())
                {
                    JournalViewModel journalRow = GetDayStatsByUser(user, records.Where(x => x.IsConfirmed).ToList(), selectedDate.Value);
                    dayStats.Add(journalRow);
                }
            }

            switch (sortOrder)
            {
                case "name":
                    dayStats = dayStats.OrderBy(s => s.User.UserInfo.Name).ToList();
                    break;
                case "come":
                    dayStats = dayStats.OrderBy(s => s.Come.Time).ToList();
                    break;
                case "gone":
                    dayStats = dayStats.OrderBy(s => s.Gone.Time).ToList();
                    break;
                case "workschedule":
                    dayStats = dayStats.OrderBy(s => s.WorkSchedule.Name).ToList();
                    break;
                case "outForWork":
                    dayStats = dayStats.OrderBy(s => s.OutForWorkTime).ToList();
                    break;
                case "byPermission":
                    dayStats = dayStats.OrderBy(s => s.ByPermissionTime).ToList();
                    break;
                case "debthWork":
                    dayStats = dayStats.OrderBy(s => s.PlusDebtWorkTime).ToList();
                    break;
                case "overWork":
                    dayStats = dayStats.OrderBy(s => s.OverWorkTime).ToList();
                    break;
                case "totalTime":
                    dayStats = dayStats.OrderBy(s => s.TotalTime).ToList();
                    break;
                case "isSystem":
                    dayStats = dayStats.OrderBy(s => s.IsSystem).ToList();
                    break;
                default:
                    dayStats = dayStats.OrderBy(s => s.User.UserInfo.Name).ToList();
                    break;
            }

            model.DateStats = dayStats;
            ViewBag.SelectedDate = selectedDate;
            ViewBag.PreviousDate = selectedDate.Value.AddDays(-1);
            ViewBag.NextDate = selectedDate.Value.AddDays(1);

            
            return View(model);
        }

        public ActionResult Create(DateTime? selectedDate, string userId = "")
        {
            Record record = new Record();
            record.DateRecord = selectedDate ?? DateTime.Now;
            record.TimeRecord = DateTime.Now.TimeOfDay;
            record.Status = (int)Statuses.Come;
            record.Remark = (int)Remarks.ComeGone;
            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo).OrderBy(x => x.Name).ToList();
                ViewBag.Users = new SelectList(userInfoes, "UserId", "Name", userId);
                ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name");
            }

            ViewBag.SelectedDate = selectedDate;
            ViewBag.UserId = userId;
            return View(record);
        }

        [HttpPost]
        public ActionResult Create(string UserId, Record model)
        {
            if (UserId == "")
                UserId = User.Identity.GetUserId();

            var dbUser = db.Users.Include(x => x.UserInfo).Include(x => x.UserInfo.WorkSchedule).FirstOrDefault(x => x.Id == UserId);
            if (dbUser != null)
            {
                string key = "";
                if (dbUser.UserInfo != null)
                    key = dbUser.UserInfo.Key;

                if (key != "")
                {
                    if (model.Remark == (int)Remarks.DebtWork && model.DebtWorkDate == null)
                        return View(UserId, model);

                    if (model.Remark != (int)Remarks.DebtWork)
                        model.DebtWorkDate = null;

                    if (User.IsInRole("Admin"))
                        model.IsConfirmed = true;
                    else
                        model.IsConfirmed = false;

                    model.DateCreated = DateTime.Now;
                    model.User = dbUser;

                    model.IsForgiven = false;
                    model.IsSystem = false;
                    model.IsLate = false;

                    model.WorkSchedule = dbUser.UserInfo.WorkSchedule;

                    StartEndWorkViewModel startEndWork = GetSpecialSchedule(dbUser.UserInfo.WorkSchedule, model.DateRecord);

                    if (model.Status == (int)Statuses.Come && model.Remark == (int)Remarks.ComeGone)
                    {
                        if ((model.TimeRecord - startEndWork.StartTime).TotalMinutes > 6)
                            model.IsLate = true;
                    }
                    else if (model.Status == (int)Statuses.Gone && model.Remark == (int)Remarks.ComeGone)
                    {
                        if ((startEndWork.EndTime - model.TimeRecord).TotalMinutes > 6)
                            model.IsLate = true;
                    }

                    if (User.IsInRole("Employee") && UserId != User.Identity.GetUserId())
                        return RedirectToAction("Index", new { selectedDate = model.DateRecord });

                    db.Records.Add(model);
                    db.SaveChanges();
                }
            }

            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo).OrderBy(x => x.Name).ToList();
                ViewBag.UserId = new SelectList(userInfoes, "UserId", "Name", UserId);
            }

            ViewBag.SelectedDate = model.DateRecord;
            ViewBag.UserId = UserId;
            return RedirectToAction("Index", new { selectedDate = model.DateRecord, model.UserId });
        }

        public ActionResult Edit(int id)
        {
            var record = db.Records.Include(x => x.WorkSchedule).Include(x => x.User).FirstOrDefault(x => x.Id == id);

            if (User.IsInRole("Admin"))
            {
                var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo).OrderBy(x => x.Name).ToList();
                ViewBag.Users = new SelectList(userInfoes, "UserId", "Name", record.UserId);
            }
            else
            {
                if (record.UserId != User.Identity.GetUserId() || (!record.IsSystem && record.IsConfirmed == true))
                    return RedirectToAction("Index", new { selectedDate = record.DateRecord});
            }

            ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name", record.WorkSchedule.Id);
            ViewBag.UserId = record.UserId;
            ViewBag.SelectedDate = record.DateRecord;
            return View(record);
        }

        [HttpPost]
        public ActionResult Edit(Record record, DateTime selectedDate)
        {
            var dbRecord = db.Records.Find(record.Id);
            
            if (User.IsInRole("Employee") && (dbRecord.UserId != User.Identity.GetUserId() || (!dbRecord.IsSystem && dbRecord.IsConfirmed == true)))
                return RedirectToAction("Index", new { selectedDate = record.DateRecord });

            if (User.IsInRole("Admin"))
                dbRecord.IsConfirmed = true;
            else
                dbRecord.IsConfirmed = false;

            /*StartEndWorkViewModel startEndWork = GetSpecialSchedule(db.WorkSchedules.Find(dbRecord.WorkScheduleId), dbRecord.DateRecord);

            if (record.Status == (int)Statuses.Come && record.Remark == (int)Remarks.ComeGone)
            {
                if ((record.TimeRecord - startEndWork.StartTime).TotalMinutes > 6)
                    dbRecord.IsLate = true;
                else
                    dbRecord.IsLate = false;
            }
            else if (record.Status == (int)Statuses.Gone && record.Remark == (int)Remarks.ComeGone)
            {
                if ((startEndWork.EndTime - record.TimeRecord).TotalMinutes > 6)
                    dbRecord.IsLate = true;
                else
                    dbRecord.IsLate = false;
            }*/

            if (!(record.Remark == (int)Remarks.DebtWork && record.DebtWorkDate == null))
            {
                if (ModelState.IsValid)
                {
                    UpdateModel(dbRecord);
                    db.SaveChanges();
                    if (record.Remark != (int)Remarks.DebtWork)
                    {
                        dbRecord.DebtWorkDate = null;
                        //db.SaveChanges();
                    }
                    StartEndWorkViewModel startEndWork = GetSpecialSchedule(db.WorkSchedules.Find(dbRecord.WorkScheduleId), dbRecord.DateRecord);

                    if (dbRecord.Status == (int)Statuses.Come && dbRecord.Remark == (int)Remarks.ComeGone)
                    {
                        if ((dbRecord.TimeRecord - startEndWork.StartTime).TotalMinutes > 6)
                            dbRecord.IsLate = true;
                        else
                            dbRecord.IsLate = false;
                    }
                    else if (dbRecord.Status == (int)Statuses.Gone && dbRecord.Remark == (int)Remarks.ComeGone)
                    {
                        if ((startEndWork.EndTime - dbRecord.TimeRecord).TotalMinutes > 6)
                            dbRecord.IsLate = true;
                        else
                            dbRecord.IsLate = false;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", new { selectedDate = selectedDate, dbRecord.UserId });
                }
            }

            var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
            var userInfoes = db.Users.Where(x => x.Roles.Any(i => i.RoleId == roleId)).Select(x => x.UserInfo).OrderBy(x => x.Name).ToList();

            ViewBag.WorkSchedule = new SelectList(db.WorkSchedules.ToList(), "Id", "Name", dbRecord.WorkScheduleId);
            ViewBag.Users = new SelectList(userInfoes, "UserId", "Name", dbRecord.UserId);
            ViewBag.SelectedDate = selectedDate;
            ViewBag.UserId = dbRecord.UserId;
            return View(record);
        }

        public ActionResult Delete(int id)
        {
            var record = db.Records.Include(x => x.User.UserInfo).Include(x => x.WorkSchedule).FirstOrDefault(x => x.Id == id);
            if (User.IsInRole("Employee") && !record.IsSystem && record.IsConfirmed)
                return RedirectToAction("Index", new { selectedDate = record.DateRecord });

            ViewBag.SelectedDate = record.DateRecord;
            ViewBag.UserId = record.UserId;
            return View(record);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var record = db.Records.Find(id);

            if (User.IsInRole("Employee") && (record.UserId != User.Identity.GetUserId() || (!record.IsSystem && record.IsConfirmed == true)))
                return RedirectToAction("Index", new { selectedDate = record.DateRecord });

            string userId = record.UserId;
            ViewBag.UserId = userId;
            try
            {
                db.Records.Remove(record);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Невозможно удалить запись!");
                TempData["Message"] = "Невозможно удалить скидку.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", new { selectedDate = record.DateRecord, userId = userId });
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public void ConfirmAll(string date, string userId = "")
        {
            DateTime selectedDate = Convert.ToDateTime(date);
            var records = new List<Record>();
            if (userId != "")
            {
                records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate.Date && x.IsConfirmed == false && x.UserId == userId).ToList();
            }
            else
            {
                records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate.Date && x.IsConfirmed == false).ToList();
            }

            foreach (var record in records)
            {
                record.IsConfirmed = true;
                db.SaveChanges();
            }
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public void ForgiveAll(string date, string userId = "")
        {
            DateTime selectedDate = Convert.ToDateTime(date);
            var records = new List<Record>();
            if (userId != "")
            {
                records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate.Date
                                                               && x.IsForgiven == false
                                                               && x.UserId == userId
                                                               && (x.IsLate == true
                                                               || x.Remark == (int)Remarks.ByPermission))
                                                               .ToList();
            }
            else
            {

                records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == selectedDate.Date
                                                                    && x.IsForgiven == false
                                                                    && (x.IsLate == true
                                                                    || x.Remark == (int)Remarks.ByPermission))
                                                                    .ToList();
            }

            foreach (var record in records)
            {
                record.IsForgiven = true;
            }
            db.SaveChanges();
        }

        public bool UpdateDataFromFile(DateTime? selectedDate)
        {
            var setting = db.Settings.FirstOrDefault();
            if (setting != null)
            {
                string path = setting.FilePath;
                string fileText = "";
                try
                {
                    /*using (StreamReader reader = new StreamReader(System.IO.File.Open(Path.Combine(path, "reader_t.log"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        fileText = reader.ReadToEnd();
                    }*/
                    WebClient request = new WebClient();
                    request.Credentials = new NetworkCredential("anonymous", "anonymous@example.com");
                    byte[] newFileData = request.DownloadData(path);
                    fileText = System.Text.Encoding.UTF8.GetString(newFileData);
 
                }
                catch (Exception ex)
                {
                    return false;
                }

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
                                            if (user != null && user.UserInfo.WorkSchedule != null)
                                            {
                                                Record record = new Record();
                                                record.DateCreated = newRecord.Date;
                                                record.DateRecord = newRecord.Date;
                                                record.TimeRecord = newRecord.Time;
                                                record.IsConfirmed = true;
                                                record.IsSystem = true;
                                                record.IsForgiven = false;
                                                record.User = user;
                                                record.WorkSchedule = user.UserInfo.WorkSchedule;

                                                record.Remark = (int)Remarks.ComeGone;
                                                if (!db.Records.Where(x => x.DateRecord == selectedDate && x.IsSystem).Any(x => x.User.Id == user.Id))
                                                {
                                                    if ((record.WorkSchedule.EndWork - record.TimeRecord).TotalHours > 1)
                                                        record.Status = (int)Statuses.Come;
                                                    else
                                                        record.Status = (int)Statuses.Gone;
                                                }
                                                else
                                                    record.Status = (int)Statuses.Gone;

                                                TimeSpan startWork = TimeSpan.Zero;
                                                TimeSpan endWork = TimeSpan.Zero;
                                                int dayOfWeek = (int)newRecord.Date.DayOfWeek;
                                                if (!user.UserInfo.WorkSchedule.IsSpecial)
                                                {
                                                    startWork = user.UserInfo.WorkSchedule.StartWork;
                                                    endWork = user.UserInfo.WorkSchedule.EndWork;
                                                }
                                                else
                                                {
                                                    var specials = db.SpecialSchedules.Where(x => x.WorkScheduleId == user.UserInfo.WorkSchedule.Id).ToList();
                                                    if (specials.Any(x => x.DayOfWeek == dayOfWeek))
                                                    {
                                                        startWork = specials.FirstOrDefault(x => x.DayOfWeek == dayOfWeek).StartTime;
                                                        endWork = specials.FirstOrDefault(x => x.DayOfWeek == dayOfWeek).EndTime;
                                                    }
                                                    else
                                                    {
                                                        startWork = user.UserInfo.WorkSchedule.StartWork;
                                                        endWork = user.UserInfo.WorkSchedule.EndWork;
                                                    }
                                                }

                                                if (record.Status == (int)Statuses.Come)
                                                {
                                                    if ((newRecord.Time - startWork).TotalMinutes > 6)
                                                        record.IsLate = true;
                                                    else
                                                        record.IsLate = false;
                                                }
                                                else if (record.Status == (int)Statuses.Gone)
                                                {
                                                    if ((endWork - newRecord.Time).TotalMinutes > 6)
                                                        record.IsLate = true;
                                                    else
                                                        record.IsLate = false;
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
            return true;
        }

        public ApplicationUser GetUserByKey(string key)
        {
            var user = db.Users.Include(x => x.UserInfo).Include(x => x.UserInfo.WorkSchedule).FirstOrDefault(x => x.UserInfo.Key == key);
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
                    if (fileRecords[fileRecords.Count - 1].Key == fileRecord.Key && (fileRecord.Time - fileRecords[fileRecords.Count - 1].Time).TotalMinutes < 30)
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

        [Authorize(Roles = "Admin")]
        public ActionResult MonthHoursReview(DateTime? startDate, DateTime? endDate, string sortOrder = "name")
        {
            if (startDate == null || endDate == null)
            {
                DateTime currentDate = DateTime.Now;
                startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }

            DateTime previousStartDate = startDate.Value.AddMonths(-1);
            DateTime previousEndDate = previousStartDate.AddMonths(1).AddDays(-1);

            DateTime nextStartDate = startDate.Value.AddMonths(1);
            DateTime nextEndDate = nextStartDate.AddMonths(1).AddDays(-1);

            List<MonthHoursViewModel> model = GetMonthHours(startDate, endDate);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.PreviousStartDate = previousStartDate;
            ViewBag.PreviousEndDate = previousEndDate;
            ViewBag.NextStartDate = nextStartDate;
            ViewBag.NextEndDate = nextEndDate;

            switch (sortOrder)
            {
                case "name":
                    model = model.OrderBy(s => s.Name).ToList();
                    break;
                case "late":
                    model = model.OrderBy(s => s.LateHours).ToList();
                    break;
                case "forgivenLate":
                    model = model.OrderBy(s => s.LateForgivenHours).ToList();
                    break;
                case "earlyGone":
                    model = model.OrderBy(s => s.EarlyGoneHours).ToList();
                    break;
                case "forgivenEarlyGone":
                    model = model.OrderBy(s => s.EarlyGoneForgivenHours).ToList();
                    break;
                case "outForwork":
                    model = model.OrderBy(s => s.OutForWorkHours).ToList();
                    break;
                case "byPermission":
                    model = model.OrderBy(s => s.ByPermissionHours).ToList();
                    break;
                case "byPermissionForgiven":
                    model = model.OrderBy(s => s.ByPermissionForgivenHours).ToList();
                    break;
                case "debtWork":
                    model = model.OrderBy(s => s.DebtWorkHours).ToList();
                    break;
                case "overWork":
                    model = model.OrderBy(s => s.OverWorkHours).ToList();
                    break;
                case "totalHours":
                    model = model.OrderBy(s => s.TotalHours).ToList();
                    break;
                default:
                    model = model.OrderBy(s => s.Name).ToList();
                    break;
            }

            return View(model);
        }

        List<MonthHoursViewModel> GetMonthHours(DateTime? startDate, DateTime? endDate)
        {
            List<MonthHoursViewModel> model = new List<MonthHoursViewModel>();

            List<StatsViewModel> monthStats = new List<StatsViewModel>();
            var records = db.Records.Where(x => (DbFunctions.TruncateTime(x.DateRecord) >= startDate && DbFunctions.TruncateTime(x.DateRecord) <= endDate)
                                            || (DbFunctions.TruncateTime(x.DebtWorkDate) >= startDate && DbFunctions.TruncateTime(x.DebtWorkDate) <= endDate))
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();

            if (records.Any())
            {
                List<DateTime> debtWorkList = records.Where(x => x.DebtWorkDate != null).Select(x => x.DebtWorkDate.Value).ToList();
                foreach (DateTime date in records.Select(x => x.DateRecord).Union(debtWorkList).Distinct().OrderBy(x => x.Date).ToList())
                {
                    StatsViewModel dateStats = new StatsViewModel();
                    dateStats.Date = date;
                    string dateName = Helper.DaysOfWeekHelper.GetDayName((int)date.DayOfWeek);
                    dateName += " - ";
                    dateName += date.ToString("dd MMM yyyy");

                    dateStats.DateName = dateName;
                    List<JournalViewModel> stats = new List<JournalViewModel>();
                    foreach (var user in records.Select(x => x.User).Distinct().ToList())
                    {
                        var filteredRecords = records.Where(x => x.DateRecord == date || x.DebtWorkDate == date).ToList();
                        JournalViewModel userStats = GetDayStatsByUser(user, filteredRecords.Where(x => x.IsConfirmed).ToList(), date);
                        if (userStats.User != null)
                            stats.Add(userStats);

                    }
                    dateStats.DateStats = stats.OrderBy(x => x.User.UserInfo.Name).ToList();
                    monthStats.Add(dateStats);
                }
                if (monthStats.Any())
                {
                    var roleId = db.Roles.FirstOrDefault(x => x.Name == "Employee").Id;
                    var users = db.UserInfoes.Where(x => x.User.Roles.Any(i => i.RoleId == roleId)).OrderBy(x => x.Name).ToList();
                    if (users.Any())
                    {
                        int lateHours;
                        int lateForgivenHours;
                        int earlyGoneHours;
                        int earlyGoneForgivenHours;
                        TimeSpan outForWorkHours;
                        TimeSpan byPermissionHours;
                        TimeSpan byPermissionForgivenkHours;
                        TimeSpan debtWorkHours;
                        TimeSpan overWorkHours;
                        TimeSpan totalHours;

                        foreach (var user in users)
                        {
                            MonthHoursViewModel monthHours = new MonthHoursViewModel();
                            monthHours.Name = user.Name;

                            lateHours = 0;
                            lateForgivenHours = 0;
                            earlyGoneHours = 0;
                            earlyGoneForgivenHours = 0;
                            outForWorkHours = TimeSpan.Zero;
                            byPermissionHours = TimeSpan.Zero;
                            byPermissionForgivenkHours = TimeSpan.Zero;
                            debtWorkHours = TimeSpan.Zero;
                            overWorkHours = TimeSpan.Zero;
                            totalHours = TimeSpan.Zero;

                            foreach (var date in monthStats)
                            {
                                foreach (var stats in date.DateStats.Where(x => x.User == user.User))
                                {
                                    if (stats.Come.IsLate)
                                    {
                                        lateHours++;
                                        if (stats.Come.IsForgiven)
                                            lateForgivenHours++;
                                    }
                                    if (stats.Gone.IsEarlyGone)
                                    {
                                        earlyGoneHours++;
                                        if (stats.Gone.IsForgiven)
                                            earlyGoneForgivenHours++;
                                    }
                                    outForWorkHours += stats.OutForWorkTime;
                                    byPermissionHours += stats.ByPermissionTime;
                                    byPermissionForgivenkHours += stats.ByPermissionForgivenTime;
                                    debtWorkHours += stats.PlusDebtWorkTime;
                                    overWorkHours += stats.OverWorkTime;
                                    totalHours += stats.TotalTime;
                                }
                            }

                            monthHours.LateHours = lateHours;
                            monthHours.LateForgivenHours = lateForgivenHours;
                            monthHours.EarlyGoneHours = earlyGoneHours;
                            monthHours.EarlyGoneForgivenHours = earlyGoneForgivenHours;
                            monthHours.OutForWorkHours = Math.Round(outForWorkHours.TotalHours, 2);
                            monthHours.ByPermissionHours = Math.Round(byPermissionHours.TotalHours, 2);
                            monthHours.ByPermissionForgivenHours = Math.Round(byPermissionForgivenkHours.TotalHours, 2);
                            monthHours.DebtWorkHours = Math.Round(debtWorkHours.TotalHours, 2);
                            monthHours.OverWorkHours = Math.Round(overWorkHours.TotalHours, 2);
                            monthHours.TotalHours = Math.Round(totalHours.TotalHours, 2);

                            model.Add(monthHours);
                        }

                    }

                }
            }
            return model;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult MonthReview(DateTime? startDate, DateTime? endDate, bool all = true, bool onlyProblem = false, bool onlyUser = false, string sortOrder = "name")
        {
            if (startDate == null || endDate == null)
            {
                DateTime currentDate = DateTime.Now;
                startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }

            DateTime previousStartDate = startDate.Value.AddMonths(-1);
            DateTime previousEndDate = previousStartDate.AddMonths(1).AddDays(-1);

            DateTime nextStartDate = startDate.Value.AddMonths(1);
            DateTime nextEndDate = nextStartDate.AddMonths(1).AddDays(-1);

            List<StatsViewModel> model = GetMonthStats(startDate, endDate, all, onlyProblem, onlyUser, sortOrder);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.PreviousStartDate = previousStartDate;
            ViewBag.PreviousEndDate = previousEndDate;
            ViewBag.NextStartDate = nextStartDate;
            ViewBag.NextEndDate = nextEndDate;
            ViewBag.All = all;
            ViewBag.OnlyProblem = onlyProblem;
            ViewBag.OnlyUser = onlyUser;

            return View(model);
        }

        public ActionResult MonthReviewByUser(DateTime? startDate, DateTime? endDate, string userId = "", string sortOrder = "name")
        {
            if (startDate == null || endDate == null)
            {
                DateTime currentDate = DateTime.Now;
                startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }

            if (User.IsInRole("Employee"))
            {
                userId = User.Identity.GetUserId();
            }

            DateTime previousStartDate = startDate.Value.AddMonths(-1);
            DateTime previousEndDate = previousStartDate.AddMonths(1).AddDays(-1);

            DateTime nextStartDate = startDate.Value.AddMonths(1);
            DateTime nextEndDate = nextStartDate.AddMonths(1).AddDays(-1);

            List<StatsViewModel> model = GetMonthStatsByUser(startDate, endDate, userId, sortOrder);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.PreviousStartDate = previousStartDate;
            ViewBag.PreviousEndDate = previousEndDate;
            ViewBag.NextStartDate = nextStartDate;
            ViewBag.NextEndDate = nextEndDate;

            if (User.IsInRole("Admin"))
                ViewBag.User = new SelectList(db.UserInfoes.Where(x => x.Key != null).OrderBy(x => x.Name).ToList(), "UserId", "Name", userId);
            ViewBag.UserId = userId;

            return View(model);
        }

        List<StatsViewModel> GetMonthStats(DateTime? startDate, DateTime? endDate, bool all, bool onlyProblem, bool onlyUser, string sortOrder = "name")
        {
            List<StatsViewModel> model = new List<StatsViewModel>();
            var records = db.Records.Where(x => (DbFunctions.TruncateTime(x.DateRecord) >= startDate && DbFunctions.TruncateTime(x.DateRecord) <= endDate) 
                                            || (DbFunctions.TruncateTime(x.DebtWorkDate) >= startDate && DbFunctions.TruncateTime(x.DebtWorkDate) <= endDate))
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();

            if (records.Any())
            {
                for (var date = startDate; date <= endDate; date = date.Value.AddDays(1))
                {
                    if (date.Value > DateTime.Now.Date)
                        return model;

                    StatsViewModel dateStats = new StatsViewModel();
                    dateStats.Date = date.Value;
                    string dateName = Helper.DaysOfWeekHelper.GetDayName((int)date.Value.DayOfWeek);
                    dateName += " - ";
                    dateName += date.Value.ToString("dd MMM yyyy");

                    dateStats.DateName = dateName;

                    

                    List<JournalViewModel> stats = new List<JournalViewModel>();
                    foreach (var user in records.Select(x => x.User).Distinct().ToList())
                    {
                        var filteredRecords = records.Where(x => x.DateRecord == date || x.DebtWorkDate == date).ToList();

                        if (filteredRecords.Any(x => x.IsConfirmed == false && x.DateRecord == date))
                            dateStats.IsNotConfirmeds = true;

                        JournalViewModel userStats = GetDayStatsByUser(user, filteredRecords.Where(x => x.IsConfirmed).ToList(), date.Value);
                        if (userStats.User != null && userStats.IsDislplay)
                        {
                            if (all)
                                stats.Add(userStats);
                            else
                            {
                                if (onlyProblem && onlyUser)
                                {
                                    if ((userStats.Come.IsProblem == onlyProblem || userStats.Gone.IsProblem == onlyProblem) || userStats.IsSystem != onlyUser)
                                        stats.Add(userStats);
                                }
                                else
                                {
                                    if (onlyProblem)
                                    {
                                        if (userStats.Come.IsProblem == onlyProblem || userStats.Gone.IsProblem == onlyProblem)
                                            stats.Add(userStats);
                                    }
                                    else
                                    {
                                        if (userStats.IsSystem != onlyUser)
                                            stats.Add(userStats);
                                    }
                                }
                            }
                        }

                    }

                    switch (sortOrder)
                    {
                        case "name":
                            dateStats.DateStats = stats.OrderBy(x => x.User.UserInfo.Name).ToList();
                            break;
                        case "come":
                            dateStats.DateStats = stats.OrderBy(x => x.Come.Time).ToList();
                            break;
                        case "gone":
                            dateStats.DateStats = stats.OrderBy(x => x.Gone.Time).ToList();
                            break;
                        case "workschedule":
                            dateStats.DateStats = stats.OrderBy(x => x.WorkSchedule.Name).ToList();
                            break;
                        case "outForWork":
                            dateStats.DateStats = stats.OrderBy(x => x.OutForWorkTime).ToList();
                            break;
                        case "byPermission":
                            dateStats.DateStats = stats.OrderBy(x => x.ByPermissionTime).ToList();
                            break;
                        case "debthWork":
                            dateStats.DateStats = stats.OrderBy(x => x.PlusDebtWorkTime).ToList();
                            break;
                        case "overWork":
                            dateStats.DateStats = stats.OrderBy(x => x.OverWorkTime).ToList();
                            break;
                        case "totalTime":
                            dateStats.DateStats = stats.OrderBy(x => x.TotalTime).ToList();
                            break;
                        case "isSystem":
                            dateStats.DateStats = stats.OrderBy(x => x.IsSystem).ToList();
                            break;
                        default:
                            dateStats.DateStats = stats.OrderBy(x => x.User.UserInfo.Name).ToList();
                            break;
                    }

                    model.Add(dateStats);
                }
            }
            return model;
        }

        List<StatsViewModel> GetMonthStatsByUser(DateTime? startDate, DateTime? endDate, string userId = "", string sortOrder = "name")
        {
            List<StatsViewModel> model = new List<StatsViewModel>();
            var records = db.Records.Where(x => (DbFunctions.TruncateTime(x.DateRecord) >= startDate && DbFunctions.TruncateTime(x.DateRecord) <= endDate)
                                            || (DbFunctions.TruncateTime(x.DebtWorkDate) >= startDate && DbFunctions.TruncateTime(x.DebtWorkDate) <= endDate)
                                            && x.UserId == userId)
                                            .Include(x => x.WorkSchedule)
                                            .Include(x => x.User.UserInfo)
                                            .OrderBy(x => x.TimeRecord)
                                            .ToList();

            if (records.Any())
            {
                ApplicationUser user = db.Users.Find(userId);
                for(var date = startDate; date <= endDate; date = date.Value.AddDays(1))
                {
                    if (date.Value > DateTime.Now.Date)
                        return model;

                    StatsViewModel dateStats = new StatsViewModel();
                    dateStats.Date = date.Value;
                    string dateName = date.Value.ToString("dd MMM yyyy");
                    dateName += " - ";
                    dateName += Helper.DaysOfWeekHelper.GetDayName((int)date.Value.DayOfWeek);

                    dateStats.DateName = dateName;
                    
                    List<JournalViewModel> stats = new List<JournalViewModel>();
    
                    var filteredRecords = records.Where(x => x.DateRecord == date || x.DebtWorkDate == date).ToList();

                    if (filteredRecords.Any(x => x.IsConfirmed == false && x.DateRecord == date))
                        dateStats.IsNotConfirmeds = true;

                    JournalViewModel userStats = GetDayStatsByUser(user, filteredRecords.Where(x => x.IsConfirmed).ToList(), date.Value);
                    if (userStats.User != null && userStats.IsDislplay)
                        stats.Add(userStats);

                    switch (sortOrder)
                    {
                        case "name":
                            dateStats.DateStats = stats.OrderBy(x => x.User.UserInfo.Name).ToList();
                            break;
                        case "come":
                            dateStats.DateStats = stats.OrderBy(x => x.Come.Time).ToList();
                            break;
                        case "gone":
                            dateStats.DateStats = stats.OrderBy(x => x.Gone.Time).ToList();
                            break;
                        case "workschedule":
                            dateStats.DateStats = stats.OrderBy(x => x.WorkSchedule.Name).ToList();
                            break;
                        case "outForWork":
                            dateStats.DateStats = stats.OrderBy(x => x.OutForWorkTime).ToList();
                            break;
                        case "byPermission":
                            dateStats.DateStats = stats.OrderBy(x => x.ByPermissionTime).ToList();
                            break;
                        case "debthWork":
                            dateStats.DateStats = stats.OrderBy(x => x.PlusDebtWorkTime).ToList();
                            break;
                        case "overWork":
                            dateStats.DateStats = stats.OrderBy(x => x.OverWorkTime).ToList();
                            break;
                        case "totalTime":
                            dateStats.DateStats = stats.OrderBy(x => x.TotalTime).ToList();
                            break;
                        case "isSystem":
                            dateStats.DateStats = stats.OrderBy(x => x.IsSystem).ToList();
                            break;
                        default:
                            dateStats.DateStats = stats.OrderBy(x => x.User.UserInfo.Name).ToList();
                            break;
                    }

                    model.Add(dateStats);
                }
            }
            return model;
        }

        JournalViewModel GetDayStatsByUser(ApplicationUser user, List<Record> records, DateTime date)
        {
            JournalViewModel journalRow = new JournalViewModel();
            journalRow.IsDislplay = true;
            journalRow.User = user;
            journalRow.IsSystem = true;
            var filteredRecords = records.Where(x => x.User == user).ToList();
            if (filteredRecords.Any())
            {
                journalRow.WorkSchedule = filteredRecords.FirstOrDefault().WorkSchedule;
                journalRow.Come = GetJournalCome(filteredRecords, journalRow, date);
                journalRow.Gone = GetJournalGone(filteredRecords, journalRow, date);

                CountTime(filteredRecords, journalRow, date);
            }
            else if(user != null)
            {
                journalRow.WorkSchedule = db.WorkSchedules.Find(db.UserInfoes.FirstOrDefault(x => x.UserId == user.Id).WorkScheduleId);

                List<Record> emptyList = new List<Record>();

                journalRow.Come = GetJournalCome(emptyList, journalRow, date);
                journalRow.Gone = GetJournalGone(emptyList, journalRow, date);

                CountTime(emptyList, journalRow, date);

            }
            return journalRow;
        }

        public ComeViewModel GetJournalCome(List<Record> filteredRecords, JournalViewModel journalRow, DateTime date)
        {
            ComeViewModel come = new ComeViewModel
            {
                IsForgiven = false,
                IsProblem = false,
                Comment = ""
            };

            var firstCome = filteredRecords.OrderBy(x => x.TimeRecord).FirstOrDefault(x => x.DateRecord == date && x.IsConfirmed == true);
            if (firstCome != null)
            {
                int remark = firstCome.Remark;
                int status = firstCome.Status;

                if (!firstCome.IsSystem)
                    journalRow.IsSystem = false;

                if (firstCome.WithoutTimebreak)
                    journalRow.WithoutTimebreak = true;

                if (status == (int)Statuses.Gone)
                {
                    come.IsProblem = true;
                    come.Comment = "Не указано время прихода";
                }
                else
                {
                    come.Time = firstCome.TimeRecord;
                    switch (remark)
                    {
                        case (int)Remarks.ComeGone:
                            if (firstCome.IsLate)
                            {
                                come.IsProblem = true;
                                come.IsLate = true;
                                come.Comment = "Опоздание";
                            }
                            break;
                        case (int)Remarks.OutForWork:
                            come.IsProblem = true;
                            come.IsForgiven = true;
                            come.Comment = "Ушел по работе";
                            come.Time = firstCome.TimeRecord;
                            break;
                        case (int)Remarks.ByPermission:
                            come.IsProblem = true;
                            if (firstCome.IsForgiven)
                                come.IsForgiven = true;
                            come.Comment = "Отпросился";
                            break;
                        case (int)Remarks.DebtWork:
                            come.Comment = "Отработка";
                            break;
                        case (int)Remarks.OverWork:
                            come.Comment = "Переработка";
                            break;
                        default:
                            come.IsProblem = true;
                            come.Comment = "Не указано время прихода";
                            break;
                    }
                }
                
            }
            else
            {
                come.IsProblem = true;
                come.Comment = "Не указано время прихода";
            }
            return come;
        }

        public GoneViewModel GetJournalGone(List<Record> filteredRecords, JournalViewModel journalRow, DateTime date)
        {
            GoneViewModel gone = new GoneViewModel
            {
                IsForgiven = false,
                IsProblem = false,
                Comment = ""
            };

            var lastGone = filteredRecords.OrderByDescending(x => x.TimeRecord).FirstOrDefault(x => x.DateRecord == date && x.IsConfirmed == true);
            if (lastGone != null)
            {
                int remark = lastGone.Remark;
                int status = lastGone.Status;

                if (!lastGone.IsSystem)
                    journalRow.IsSystem = false;

                if (lastGone.WithoutTimebreak)
                    journalRow.WithoutTimebreak = true;

                if (status == (int)Statuses.Come)
                {
                    gone.IsProblem = true;
                    gone.Comment = "Не указано время ухода";
                }
                else
                {
                    gone.Time = lastGone.TimeRecord;
                    switch (remark)
                    {
                        case (int)Remarks.ComeGone:
                            if (lastGone.IsLate)
                            {
                                gone.IsProblem = true;
                                gone.IsEarlyGone = true;
                                gone.Comment = "Уход раньше времени";
                            }
                            break;
                        case (int)Remarks.OutForWork:
                            gone.IsProblem = true;
                            gone.IsForgiven = true;
                            gone.Comment = "Ушел по работе";
                            gone.Time = lastGone.TimeRecord;
                            break;
                        case (int)Remarks.ByPermission:
                            gone.IsProblem = true;
                            if (lastGone.IsForgiven)
                                gone.IsForgiven = true;
                            gone.Comment = "Отпросился";
                            break;
                        case (int)Remarks.DebtWork:
                            gone.Comment = "Отработка";
                            break;
                        case (int)Remarks.OverWork:
                            gone.Comment = "Переработка";
                            break;
                        default:
                            gone.IsProblem = true;
                            gone.Comment = "Не указано время ухода";
                            break;
                    }
                }

            }
            else
            {
                gone.IsProblem = true;
                gone.Comment = "Не указано время ухода";
            }

            return gone;
        }

        public void CountTime(List<Record> filteredRecords, JournalViewModel journalRow, DateTime date)
        {
            StartEndWorkViewModel startEnd = GetSpecialSchedule(journalRow.WorkSchedule, date);
            TimeSpan endWorkTime = startEnd.EndTime;
            
            journalRow.OutForWorkTime = CountOutForWorkTime(filteredRecords, startEnd);
            journalRow.ByPermissionTime = CountByPermissionTime(filteredRecords, startEnd);
            journalRow.ByPermissionForgivenTime = CountByPermissionForgivenTime(filteredRecords, startEnd);
            journalRow.MinusDebtWorkTime = CountMinusDebtWorkTime(filteredRecords, endWorkTime, date);
            journalRow.PlusDebtWorkTime = CountPlusDebtWorkTime(filteredRecords, endWorkTime, date);
            journalRow.OverWorkTime = CountOverWorkTime(filteredRecords, endWorkTime);
            journalRow.TotalTime = CountComeGoneTime(filteredRecords, startEnd) + journalRow.PlusDebtWorkTime;

           // journalRow.TotalTime = CountComeGoneTime(filteredRecords, startEnd) - journalRow.ByPermissionTime*/ /*- journalRow.MinusDebtWorkTime*/ + journalRow.PlusDebtWorkTime /*+ journalRow.OverWorkTime*/;


            TimeSpan totalDayTime = GetTotalTime(journalRow.WorkSchedule.StartWork, endWorkTime, startEnd.WithoutTimeBreak);

            if (journalRow.TotalTime > totalDayTime)
            {
                journalRow.TotalTime = totalDayTime;
            }
            else if (journalRow.TotalTime < TimeSpan.Zero)
            {
                journalRow.TotalTime = TimeSpan.Zero;
            }
            if (startEnd.IsWorkDay)
                journalRow.NotWorkedTime = totalDayTime - journalRow.TotalTime;
            else
                journalRow.NotWorkedTime = TimeSpan.Zero;

            if (!startEnd.IsWorkDay && journalRow.OutForWorkTime == TimeSpan.Zero &&
                journalRow.ByPermissionTime == TimeSpan.Zero &&
                journalRow.ByPermissionForgivenTime == TimeSpan.Zero &&
                journalRow.MinusDebtWorkTime == TimeSpan.Zero &&
                journalRow.PlusDebtWorkTime == TimeSpan.Zero &&
                journalRow.OverWorkTime == TimeSpan.Zero &&
                journalRow.TotalTime == TimeSpan.Zero)
            {
                journalRow.IsDislplay = false;
            }
        }

        public StartEndWorkViewModel GetSpecialSchedule(WorkSchedule workSchedule, DateTime date)
        {
            StartEndWorkViewModel model = new StartEndWorkViewModel();
            bool isWorkkDay = false;

            var specials = db.SpecialSchedules.Where(x => x.WorkScheduleId == workSchedule.Id).ToList();
            int dayOfWeek = (int)date.DayOfWeek;
            if (specials.Any(x => x.DayOfWeek == dayOfWeek))
            {
                var special = specials.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
                model.StartTime = special.StartTime;
                model.EndTime = special.EndTime;
                model.WithoutTimeBreak = special.WithoutTimeBreak;
                isWorkkDay = true;
            }
            else
            {
                model.StartTime = workSchedule.StartWork;
                model.EndTime = workSchedule.EndWork;
                model.WithoutTimeBreak = workSchedule.WithoutTimeBreak;
                if (dayOfWeek != 0 && dayOfWeek != 6)
                    isWorkkDay = true;
            }
            model.IsWorkDay = isWorkkDay;

            return model;
        }

        public TimeSpan CountComeGoneTime(List<Record> filteredRecords, StartEndWorkViewModel startEnd)
        {
            TimeSpan comeGoneTime = TimeSpan.Zero;
            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan endTime = TimeSpan.Zero;
            bool withoutTimeBreak = false;
            if (startEnd.WithoutTimeBreak)
                withoutTimeBreak = true;

            var comeComeGone = filteredRecords.FirstOrDefault(x => x.Status == (int)Statuses.Come && x.Remark == (int)Remarks.ComeGone && x.IsConfirmed);
            var goneComeGone = filteredRecords.FirstOrDefault(x => x.Status == (int)Statuses.Gone && x.Remark == (int)Remarks.ComeGone && x.IsConfirmed);

            if (comeComeGone != null)
            {
                if (comeComeGone.WithoutTimebreak)
                    withoutTimeBreak = true;
                if (comeComeGone.TimeRecord < startEnd.StartTime || (comeComeGone.TimeRecord - startEnd.StartTime).TotalMinutes < 6)
                    startTime = startEnd.StartTime;
                else if (comeComeGone.IsLate && comeComeGone.IsForgiven)
                    startTime = startEnd.StartTime;
                else if (comeComeGone.IsLate && (comeComeGone.TimeRecord - startEnd.StartTime).TotalHours < 1)
                    startTime = startEnd.StartTime + new TimeSpan(1, 0, 0);
                else
                    startTime = comeComeGone.TimeRecord;
            }
            else
            {
                var firstCome = filteredRecords.OrderBy(x => x.TimeRecord).FirstOrDefault(x => x.IsConfirmed);
                if (firstCome != null)
                {
                    switch (firstCome.Remark)
                    {
                        case (int)Remarks.OutForWork:
                            if (firstCome.Status == (int)Statuses.Come)
                                startTime = startEnd.StartTime;
                            else if (goneComeGone == null)
                                startTime = startEnd.StartTime;
                            break;
                        case (int)Remarks.ByPermission:
                            if (firstCome.Status == (int)Statuses.Come)
                            {
                                if (firstCome.IsForgiven)
                                    startTime = startEnd.StartTime;
                                else
                                    startTime = firstCome.TimeRecord;
                            }
                            break;
                        default:
                            if (firstCome.Status == (int)Statuses.Come)
                                startTime = firstCome.TimeRecord;
                            break;
                    }
                }
            }
            
            if (goneComeGone != null)
            {
                if (goneComeGone.WithoutTimebreak)
                    withoutTimeBreak = true;
                if (goneComeGone.TimeRecord > startEnd.EndTime || (startEnd.EndTime - goneComeGone.TimeRecord).TotalMinutes < 6)
                    endTime = startEnd.EndTime;
                else if (goneComeGone.IsLate && goneComeGone.IsForgiven)
                    endTime = startEnd.EndTime;
                else
                    endTime = goneComeGone.TimeRecord;
            }
            else
            {
                var lastGone = filteredRecords.OrderBy(x => x.TimeRecord).LastOrDefault(x => x.IsConfirmed);
                if (lastGone != null)
                {
                    switch (lastGone.Remark)
                    {
                        case (int)Remarks.OutForWork:
                            if (lastGone.Status == (int)Statuses.Gone)
                                endTime = startEnd.EndTime;
                            else if(comeComeGone == null)
                                endTime = startEnd.EndTime;
                            break;
                        case (int)Remarks.ByPermission:
                            if (lastGone.Status == (int)Statuses.Gone)
                            {
                                if (lastGone.IsForgiven)
                                    endTime = startEnd.EndTime;
                                else
                                    endTime = lastGone.TimeRecord;
                            }
                            break;
                        default:
                            if (lastGone.Status == (int)Statuses.Gone)
                                endTime = lastGone.TimeRecord;
                            break;
                    }
                }
            }

            comeGoneTime = GetTotalTime(startTime, endTime, withoutTimeBreak);

            var byPermissonRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.ByPermission && !x.IsForgiven && x.TimeRecord > startTime && x.TimeRecord < endTime).ToList();
            TimeSpan byPermissionTime = TimeSpan.Zero;
            if (byPermissonRecords.Any())
            {
                byPermissionTime = CountByPermissionTime(byPermissonRecords, startEnd);
            }
            return comeGoneTime - byPermissionTime;
        }

        public TimeSpan CountOutForWorkTime(List<Record> filteredRecords, StartEndWorkViewModel startEnd)
        {
            TimeSpan outForWorkTime = TimeSpan.Zero;
            var firstRecord = filteredRecords.OrderBy(x => x.TimeRecord).FirstOrDefault();
            if (firstRecord != null)
            {
                if (firstRecord.Remark == (int)Remarks.OutForWork && firstRecord.Status == (int)Statuses.Come)
                {
                    outForWorkTime += firstRecord.TimeRecord - startEnd.StartTime;
                }
            }
            
            var outForWorkRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.OutForWork && x.IsConfirmed).OrderBy(x => x.TimeRecord).ToList();
            
            if (outForWorkRecords.Any())
            {
                foreach (var goneOutForWork in outForWorkRecords.Where(x => x.Status == (int)Statuses.Gone))
                {
                    TimeSpan goneOutForWorkTime = goneOutForWork.TimeRecord;
                    var comeOutForWork = outForWorkRecords.FirstOrDefault(x => x.Status == (int)Statuses.Come && x.TimeRecord > goneOutForWorkTime);
                    if (comeOutForWork != null)
                        outForWorkTime += GetTotalTime(goneOutForWorkTime, comeOutForWork.TimeRecord, comeOutForWork.WithoutTimebreak);
                    else
                        outForWorkTime += GetTotalTime(goneOutForWorkTime, startEnd.EndTime, goneOutForWork.WithoutTimebreak);
                }
            }

            return outForWorkTime;
        }

        public TimeSpan CountByPermissionTime(List<Record> filteredRecords, StartEndWorkViewModel startEnd)
        {
            TimeSpan byPermissionTime = TimeSpan.Zero;

            var firstRecord = filteredRecords.OrderBy(x => x.TimeRecord).FirstOrDefault();
            if (firstRecord != null)
            {
                if (firstRecord.Remark == (int)Remarks.ByPermission && firstRecord.Status == (int)Statuses.Come && !firstRecord.IsForgiven)
                {
                    byPermissionTime += firstRecord.TimeRecord - startEnd.StartTime;
                }
            }

            var byPermissionRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.ByPermission && x.IsConfirmed && !x.IsForgiven).OrderBy(x => x.TimeRecord).ToList();
            
            if (byPermissionRecords.Any())
            {
                foreach (var goneByPermission in byPermissionRecords.Where(x => x.Status == (int)Statuses.Gone))
                {
                    TimeSpan goneByPermissionTime = goneByPermission.TimeRecord;
                    var comeByPermission = byPermissionRecords.FirstOrDefault(x => x.Status == (int)Statuses.Come && x.TimeRecord > goneByPermissionTime);
                    if (comeByPermission != null)
                        byPermissionTime += GetTotalTime(goneByPermissionTime, comeByPermission.TimeRecord, goneByPermission.WithoutTimebreak);
                    else
                        byPermissionTime += GetTotalTime(goneByPermissionTime, startEnd.EndTime, goneByPermission.WithoutTimebreak);
                }
            }
            return byPermissionTime;
        }

        public TimeSpan CountByPermissionForgivenTime(List<Record> filteredRecords, StartEndWorkViewModel startEnd)
        {
            TimeSpan byPermissionForgivenTime = TimeSpan.Zero;
            var firstRecord = filteredRecords.OrderBy(x => x.TimeRecord).FirstOrDefault();
            if (firstRecord != null)
            {
                if (firstRecord.Remark == (int)Remarks.ByPermission && firstRecord.Status == (int)Statuses.Come && firstRecord.IsForgiven)
                {
                    byPermissionForgivenTime += firstRecord.TimeRecord - startEnd.StartTime;
                }
            }

            var byPermissionForgivenRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.ByPermission && x.IsConfirmed && x.IsForgiven).OrderBy(x => x.TimeRecord).ToList();
            
            if (byPermissionForgivenRecords.Any())
            {
                foreach (var goneByPermissionForgiven in byPermissionForgivenRecords.Where(x => x.Status == (int)Statuses.Gone))
                {
                    TimeSpan goneByPermissionForgivenTime = goneByPermissionForgiven.TimeRecord;
                    var comeByPermissionForgiven = byPermissionForgivenRecords.FirstOrDefault(x => x.Status == (int)Statuses.Come && x.TimeRecord > goneByPermissionForgivenTime);
                    if (comeByPermissionForgiven != null)
                        byPermissionForgivenTime += GetTotalTime(goneByPermissionForgivenTime, comeByPermissionForgiven.TimeRecord, goneByPermissionForgiven.WithoutTimebreak);
                    else
                        byPermissionForgivenTime += GetTotalTime(goneByPermissionForgivenTime, startEnd.EndTime, goneByPermissionForgiven.WithoutTimebreak);
                }
            }
            return byPermissionForgivenTime;
        }

        public TimeSpan CountMinusDebtWorkTime(List<Record> filteredRecords, TimeSpan endWorkTime, DateTime date)
        {
            TimeSpan debtWorkTime = TimeSpan.Zero;
            bool withoutTimeBreak = false;
            var debtWorkRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.DebtWork && x.IsConfirmed && x.DebtWorkDate != date).OrderBy(x => x.TimeRecord).ToList();
            if (debtWorkRecords.Any())
            {
                var dates = debtWorkRecords.Select(x => x.DateRecord).Distinct().ToList();
                foreach (DateTime dt in dates)
                {
                    foreach (var debtWork in debtWorkRecords.Where(x => x.Status == (int)Statuses.Come && x.DateRecord == dt))
                    {
                        TimeSpan comeDebtWorkTime = debtWork.TimeRecord;
                        withoutTimeBreak = debtWork.WithoutTimebreak;
                        var goneDebtWork = debtWorkRecords.FirstOrDefault(x => x.Status == (int)Statuses.Gone && x.TimeRecord > comeDebtWorkTime && x.DateRecord == dt);
                        if (goneDebtWork != null)
                            debtWorkTime += GetTotalTime(comeDebtWorkTime, goneDebtWork.TimeRecord, withoutTimeBreak);
                        else
                            debtWorkTime += GetTotalTime(comeDebtWorkTime, endWorkTime, withoutTimeBreak);
                    }
                }
            }
            return debtWorkTime;
        }

        public TimeSpan CountPlusDebtWorkTime(List<Record> filteredRecords, TimeSpan endWorkTime, DateTime date)
        {
            TimeSpan debtWorkTime = TimeSpan.Zero;
            bool withoutTimeBreak = false;
            var debtWorkRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.DebtWork && x.IsConfirmed && x.DebtWorkDate == date).OrderBy(x => x.TimeRecord).ToList();
            if (debtWorkRecords.Any())
            {
                var dates = debtWorkRecords.Select(x => x.DateRecord).Distinct().ToList();
                foreach (DateTime dt in dates)
                {
                    foreach (var debtWork in debtWorkRecords.Where(x => x.Status == (int)Statuses.Come && x.DateRecord == dt))
                    {
                        withoutTimeBreak = debtWork.WithoutTimebreak;
                        TimeSpan comeDebtWorkTime = debtWork.TimeRecord;
                        var goneDebtWork = debtWorkRecords.FirstOrDefault(x => x.Status == (int)Statuses.Gone && x.TimeRecord > comeDebtWorkTime && x.DateRecord == dt);
                        if (goneDebtWork != null)
                            debtWorkTime += GetTotalTime(comeDebtWorkTime, goneDebtWork.TimeRecord, withoutTimeBreak);
                        else
                            debtWorkTime += GetTotalTime(comeDebtWorkTime, endWorkTime, withoutTimeBreak);
                    }
                }
            }
                
            return debtWorkTime;
        }

        public TimeSpan CountOverWorkTime(List<Record> filteredRecords, TimeSpan endWorkTime)
        {
            var overWorkRecords = filteredRecords.Where(x => x.Remark == (int)Remarks.OverWork && x.IsConfirmed).OrderBy(x => x.TimeRecord).ToList();
            TimeSpan overWorkTime = TimeSpan.Zero;
            if (overWorkRecords.Any())
            {
                foreach (var comeOverWork in overWorkRecords.Where(x => x.Status == (int)Statuses.Come))
                {
                    TimeSpan comeOverWorkTime = comeOverWork.TimeRecord;
                    var goneOverWork = overWorkRecords.FirstOrDefault(x => x.Status == (int)Statuses.Gone && x.TimeRecord > comeOverWorkTime);
                    if (goneOverWork != null)
                        overWorkTime += GetTotalTime(comeOverWorkTime, goneOverWork.TimeRecord, comeOverWork.WithoutTimebreak);
                }
            }
            return overWorkTime;
        }

        public TimeSpan GetTotalTime(TimeSpan startTime, TimeSpan endTime, bool timeBreak = false)
        {
            TimeSpan totalTime = TimeSpan.Zero;
            if (startTime != totalTime && endTime != totalTime && startTime < endTime)
            {
                if (!timeBreak)
                {
                    TimeSpan startBreakTime = new TimeSpan(13, 0, 0);
                    TimeSpan endBreakTime = new TimeSpan(14, 0, 0);

                    if (startTime <= startBreakTime && endTime >= endBreakTime)
                    {
                        totalTime += (startBreakTime - startTime);
                        totalTime += (endTime - endBreakTime);
                    }
                    else if (startTime <= startBreakTime && endTime <= startBreakTime)
                        totalTime = endTime - startTime;
                    else if (startTime <= startBreakTime && endTime >= startBreakTime && endTime <= endBreakTime)
                        totalTime = startBreakTime - startTime;
                    else if (startTime >= startBreakTime && startTime <= endBreakTime)
                        totalTime = endTime - endBreakTime;
                    else if (startTime >= endBreakTime)
                        totalTime = endTime - startTime;
                }
                else
                    totalTime = endTime - startTime;
            }
            
            return totalTime;
        }

        public void UpdateDataFromFileByPeriod(string startDate, string endDate)
        {
            DateTime stDFate = DateTime.Parse(startDate);
            DateTime edDate = DateTime.Parse(endDate);
            bool fileStatus = true;
            for (DateTime date = stDFate; date <= edDate;)
            {
                fileStatus = UpdateDataFromFile(date);
                date = date.AddDays(1);
            }
        }

        public ActionResult ExportFile(DateTime startDate, DateTime endDate)
        {
            StringBuilder fileText = new StringBuilder();
            string fileName = "Журнал_";
            List<StatsViewModel> records = GetMonthStats(startDate, endDate, true, false, false);

            if (records.Any())
            {
                List<FileRecordViewModel> fileRecords = new List<FileRecordViewModel>();
                fileName += records[0].Date.ToString("MM.yyyy");
                TimeSpan startTime;
                TimeSpan endTime;

                foreach (var date in records.OrderBy(x => x.Date))
                {
                    foreach (var key in date.DateStats)
                    {
                        startTime = TimeSpan.Zero;
                        endTime = TimeSpan.Zero;

                        TimeSpan startBreakTime = new TimeSpan(13, 0, 0);
                        TimeSpan endBreakTime = new TimeSpan(14, 0, 0);

                        if (key.Come.Time != TimeSpan.Zero)
                        {
                            FileRecordViewModel fileRecordCome = new FileRecordViewModel();
                            fileRecordCome.Date = date.Date;
                            startTime = GetSpecialSchedule(key.WorkSchedule, date.Date).StartTime;
                            fileRecordCome.Time = startTime;
                            fileRecordCome.Key = key.User.UserInfo.Key;
                            fileRecords.Add(fileRecordCome);
                        }

                        if (key.Gone.Time != TimeSpan.Zero)
                        {
                            FileRecordViewModel fileRecordGone = new FileRecordViewModel();
                            fileRecordGone.Date = date.Date;
                            endTime = startTime + key.TotalTime;
                            if (key.Come.Time < startBreakTime && key.Gone.Time > endBreakTime)
                                endTime += new TimeSpan(1, 0, 0);
                            fileRecordGone.Time = endTime;
                            fileRecordGone.Key = key.User.UserInfo.Key;
                            fileRecords.Add(fileRecordGone);
                        }
                    }
                }

                foreach (var record in fileRecords.OrderBy(x => x.Date).ThenBy(x => x.Time))
                {
                    fileText.Append(record.Date.ToString("dd.MM.yyyy"));
                    fileText.Append('\t');
                    fileText.Append(record.Time.ToString());
                    fileText.Append('\t');
                    fileText.Append(record.Key);
                    fileText.Append(Environment.NewLine);
                }
            }
            
            return File(Encoding.UTF8.GetBytes(fileText.ToString()),
                 "text/plain", string.Format("{0}.txt", fileName));

        }

        public void CreateOutForWork(string userId, DateTime date)
        {
            ApplicationUser user = db.Users.Find(userId);
            WorkSchedule workSchedule = db.WorkSchedules.Find(db.UserInfoes.FirstOrDefault(x => x.UserId == user.Id).WorkScheduleId);
            StartEndWorkViewModel startEnd = GetSpecialSchedule(workSchedule, date);

            Record startOutForWorkRecord = new Record();
            startOutForWorkRecord.User = user;
            startOutForWorkRecord.UserId = user.Id;
            startOutForWorkRecord.DateCreated = DateTime.Now;
            startOutForWorkRecord.DateRecord = date;
            startOutForWorkRecord.TimeRecord = startEnd.StartTime;
            startOutForWorkRecord.IsForgiven = true;
            startOutForWorkRecord.IsLate = false;
            startOutForWorkRecord.IsSystem = false;
            startOutForWorkRecord.Status = (int)Statuses.Come;
            startOutForWorkRecord.Remark = (int)Remarks.OutForWork;
            startOutForWorkRecord.WorkSchedule = workSchedule;
            startOutForWorkRecord.WorkScheduleId = workSchedule.Id;
            startOutForWorkRecord.WithoutTimebreak = false;

            Record endOutForWorkRecord = new Record();
            endOutForWorkRecord.User = user;
            endOutForWorkRecord.UserId = user.Id;
            endOutForWorkRecord.DateCreated = DateTime.Now;
            endOutForWorkRecord.DateRecord = date;
            endOutForWorkRecord.TimeRecord = startEnd.EndTime;
            endOutForWorkRecord.IsForgiven = true;
            endOutForWorkRecord.IsLate = false;
            endOutForWorkRecord.IsSystem = false;
            endOutForWorkRecord.Status = (int)Statuses.Gone;
            endOutForWorkRecord.Remark = (int)Remarks.OutForWork;
            endOutForWorkRecord.WorkSchedule = workSchedule;
            endOutForWorkRecord.WorkScheduleId = workSchedule.Id;
            endOutForWorkRecord.WithoutTimebreak = false;

            if (User.IsInRole("Admin"))
            {
                startOutForWorkRecord.IsConfirmed = true;
                endOutForWorkRecord.IsConfirmed = true;
            }
            else
            {
                startOutForWorkRecord.IsConfirmed = false;
                endOutForWorkRecord.IsConfirmed = false;
            }

            db.Records.Add(startOutForWorkRecord);
            db.Records.Add(endOutForWorkRecord);

            db.SaveChanges();
        }

    }
}
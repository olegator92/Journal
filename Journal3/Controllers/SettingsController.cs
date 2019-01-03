using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journal3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private ApplicationDbContext db = null;

        public SettingsController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Settings
        public ActionResult Index(string message = "")
        {
            var setting = db.Settings.FirstOrDefault();
            ViewBag.Message = message;
            return View(setting);
        }

        [HttpPost]
        public ActionResult Index(Setting settings, string message = "")
        {
            var dbSetting = db.Settings.FirstOrDefault();
            if (dbSetting != null)
            {
                dbSetting.FilePath = settings.FilePath;
                dbSetting.AllowEditVacation = settings.AllowEditVacation;
            }
            else
            {
                Setting newSetting = new Setting
                {
                    FilePath = settings.FilePath,
                    AllowEditVacation = false
                };
                db.Settings.Add(newSetting);
            }
            try
            {
                db.SaveChanges();
                message = "Настройки сохранены!";
            }
            catch (Exception ex)
            {
                message = "Ошибка при сохранении: " + ex.Message;
            }
            
            return RedirectToAction("Index", new { message = message });
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

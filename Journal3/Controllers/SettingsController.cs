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
        public ActionResult Index()
        {
            var setting = db.Settings.FirstOrDefault();
            return View(setting);
        }

        [HttpPost]
        public ActionResult Index(string filePath)
        {
            if (filePath != "")
            {
                var dbSetting = db.Settings.FirstOrDefault();
                if (dbSetting != null)
                {
                    dbSetting.FilePath = filePath;
                }
                else
                {
                    Setting newSetting = new Setting
                    {
                        FilePath = filePath
                    };
                    db.Settings.Add(newSetting);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Settings/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Settings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Settings/Create
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

        // GET: Settings/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Settings/Edit/5
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

        // GET: Settings/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Settings/Delete/5
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

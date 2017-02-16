using Journal3.Models;
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
        public ActionResult Index()
        {
            
            DateTime dateNow = DateTime.Now.Date;
            var records = db.Records.Where(x => DbFunctions.TruncateTime(x.DateRecord) == dateNow).ToList();
            ViewBag.DateNow = dateNow;
            return View(records);
        }


        public ActionResult Create()
        {
            Record record = new Record();
            return View(record);
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
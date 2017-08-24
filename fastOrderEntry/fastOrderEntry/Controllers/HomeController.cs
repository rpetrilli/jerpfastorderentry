using fastOrderEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fastOrderEntry.Controllers
{
    public class HomeController : Controller
    {
        private PetLineContext db = new PetLineContext();
        public ActionResult Index()
        {
            ViewBag.page = "home";
            return View();
        }        
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

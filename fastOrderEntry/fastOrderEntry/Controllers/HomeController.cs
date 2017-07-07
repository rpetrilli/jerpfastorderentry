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
            return View();
        }

        public ActionResult Articoli()
        {
            return View();
        }

        public ActionResult Listini()
        {
            return View();
        }

        public ActionResult OrdiniAcquisto()
        {
            return View();
        }

        public ActionResult OrdiniVendita()
        {          
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

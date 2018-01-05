using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fastOrderEntry.Controllers
{
    public class MovClienteController : Controller
    {
        // GET: MovCliente
        public ActionResult Index()
        {
            ViewBag.page = "movimenti";
            return View();
        }
    }
}
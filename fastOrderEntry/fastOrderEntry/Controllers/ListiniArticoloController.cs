using fastOrderEntry.Helpers;
using fastOrderEntry.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fastOrderEntry.Controllers
{
    public class ListiniArticoloController : Controller
    {
        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public ListiniArticoloController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        // GET: ListiniArticolo
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetElenco(string cerca)
        {
            con.Open();

            Listino listino = new Listino();
            listino.leggiPrezzi(con, cerca != null ? cerca.ToUpper() : "");

  

            var jsonResult = Json(new { data = listino }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        protected override void Dispose(bool disposing)
        {           
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
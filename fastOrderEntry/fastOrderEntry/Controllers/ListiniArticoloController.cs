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
            string minuscolo = cerca != null ? cerca.ToLower() : cerca;
            string maiuscolo = cerca != null ? cerca.ToUpper() : cerca;

            var query = db.ma_articoli_soc.Where(x => x.id_codice_art.Contains(cerca) | x.descrizione.Contains(minuscolo) | x.descrizione.Contains(maiuscolo));
            List<RecordListino> lista = new List<RecordListino>();

            foreach(var x in query)
            {
                RecordListino item = new RecordListino{ id_codice_art = x.id_codice_art , descrizione = x.descrizione};
                item.leggiPrezzi(con);
                lista.Add(item);
            }

            var jsonResult = Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        protected override void Dispose(bool disposing)
        {           
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
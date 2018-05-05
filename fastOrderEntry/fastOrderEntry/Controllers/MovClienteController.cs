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
    public class MovClienteController : Controller
    {
        private const int REC_X_PAGINA = 15;

        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public MovClienteController()
        {
            con = DbUtils.GetDefaultConnection();
        }
        
        public ActionResult Index()
        {
            ViewBag.page = "movimenti";
            return View();
        }

        [HttpGet]
        public JsonResult GetPaginatore(string query, string cod_cat_merc)
        {
            query = string.IsNullOrEmpty(query) ? string.Empty : query.ToUpper();
            con.Open();

            ClientiStrutturaModel clienti = new ClientiStrutturaModel();
            clienti.select(con, query);
            int cnt = clienti.rs.Count();

            var jsonResult = Json(new { rec_number =cnt , rec_x_pagina = REC_X_PAGINA, pag_number = Math.Ceiling(1.0 * cnt / REC_X_PAGINA) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetConenutoPagina(string query, string cod_cat_merc, string id_cliente, int page_number)
        {
            con.Open();
            ClientiStrutturaModel clienti = new ClientiStrutturaModel();
            clienti.select(con, query, page_number, REC_X_PAGINA);

            var jsonResult = Json(clienti.rs.OrderBy(x => x.name), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpPost]
        public JsonResult leggi_movimenti(RecordClienteModel model)
        {
            DateTime data_da = new DateTime(DateTime.Now.Year - 1, 1, 1);
            DateTime data_a = new DateTime(DateTime.Now.Year, 12, 31);

            if (!string.IsNullOrEmpty(model.da_data))
                data_da = Convert.ToDateTime(model.da_data);

            if (!string.IsNullOrEmpty(model.a_data))
            {
                data_a = Convert.ToDateTime(model.a_data);
                data_a = data_a.AddMonths(1).AddDays(-1);
            }


            ListaMovClienteModel mov_cliente = new ListaMovClienteModel();
            mov_cliente.id_cliente = model.id_cliente;
            mov_cliente.ragione_sociale = model.ragione_sociale;
            mov_cliente.select(con, data_da, data_a);

            return Json(mov_cliente, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult dettaglio_movimento(MovClienteModel model)
        {
            ListaMovimenti movimento = new ListaMovimenti();
            movimento.tipo = model.tipo;
            movimento.esercizio = model.esercizio;
            movimento.id_documento = model.id;
            movimento.select(con);
            return Json(movimento, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
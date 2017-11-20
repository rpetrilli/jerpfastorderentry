using fastOrderEntry.Helpers;
using fastOrderEntry.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static fastOrderEntry.Models.ProvvigioniModel;

namespace fastOrderEntry.Controllers
{
    public class ProvvigioniController : Controller
    {
        private const int REC_X_PAGINA = 15;

        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public ProvvigioniController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        public ActionResult Index()
        {
            ViewBag.page = "anagrafiche";
            return View();
        }

        public JsonResult GetPaginatore(string query, string cod_cat_merc)
        {
            query = string.IsNullOrEmpty(query) ? string.Empty : query.ToUpper();

            con.Open();

            int cnt = 0;

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT count(*) as cnt from ma_articoli_soc \r\n" +
                    "where id_societa = '1' \r\n" +
                    "and (upper(id_codice_art) LIKE( @query) or upper(descrizione) like( @query ) ) \r\n";
                if (!string.IsNullOrEmpty(cod_cat_merc))
                {
                    cmd.CommandText += " and (id_categoria_merc like ('" + cod_cat_merc + "-%') or id_categoria_merc ='" + cod_cat_merc + "')";
                }
                cmd.Parameters.AddWithValue("query", query + "%");
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cnt = Convert.ToInt32(reader["cnt"]);
                    }
                }
            }

            var jsonResult = Json(new { rec_number = cnt, rec_x_pagina = REC_X_PAGINA, pag_number = Math.Ceiling(1.0 * cnt / REC_X_PAGINA) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        public JsonResult GetConenutoPagina(string query, string cod_cat_merc, string id_agente, int page_number)
        {
            con.Open();

            ProvvigioniModel proviggioni = new ProvvigioniModel();
            proviggioni.select(con, query, cod_cat_merc, id_agente, page_number, REC_X_PAGINA);



            var jsonResult = Json(proviggioni.recordprovvigione, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpPost]
        public JsonResult saveProvvigione(RecordProvvigioneModel item)
        {
            con.Open();
            item.ScriviProvvigione(con);
            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult copia_provvigione(decimal valore_massivo, string query, string cod_cat_merc, string id_agente)        {
            
            con.Open();

            ProvvigioniModel provvigioni = new ProvvigioniModel();
            provvigioni.update_massivo_provvigione(con, valore_massivo, query, cod_cat_merc, id_agente);

            //TODO: copia listino

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
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
    public class ArticoliAgenteController : Controller
    {
        private const int REC_X_PAGINA = 15;

        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public ArticoliAgenteController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        public ActionResult Index()
        {
            ViewBag.page = "agenti";
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

            ArticoliAgenteModel articoliAgente = new ArticoliAgenteModel();
            articoliAgente.select(con, query, cod_cat_merc, id_agente, page_number, REC_X_PAGINA);

            var jsonResult = Json(articoliAgente.recordArticoli.OrderBy(x=> x.descrizione), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpPost]
        public JsonResult Save (RecordArticoliAgenteModel item)
        {
            con.Open();
            item.ScriviVisibile(con);
            con.Close();
            return Json(new { ack = "Ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Massivo (bool visibile, string id_agente, string query, string cod_cat_merc)
        {
            con.Open();
            ArticoliAgenteModel articoliAgente = new ArticoliAgenteModel();
            articoliAgente.update_massivo(con, visibile, id_agente, query, cod_cat_merc);
            con.Close();
            return Json(new { ack = "Ok" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
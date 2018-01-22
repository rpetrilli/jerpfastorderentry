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
    public class MovArticoloController : Controller
    {
        private const int REC_X_PAGINA = 15;

        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public MovArticoloController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        public ActionResult Index()
        {
            ViewBag.page = "movimenti";
            return View();
        }

        [HttpGet]
        public JsonResult GetElenco(string cerca)
        {
            con.Open();

            ListinoModel listino = new ListinoModel();
            listino.select(con, cerca != null ? cerca.ToUpper() : "");

            var jsonResult = Json(new { data = listino }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetCategorie()
        {
            con.Open();

            CategorieStrutturaModel categorie = new CategorieStrutturaModel();
            categorie.select(con);

            var jsonResult = Json(categorie.categorie, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpGet]
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

        [HttpGet]
        public JsonResult GetConenutoPagina(string query, string cod_cat_merc, int page_number)
        {
            con.Open();

            ListinoModel listino = new ListinoModel();
            listino.select(con, query, cod_cat_merc, page_number, REC_X_PAGINA);


            var jsonResult = Json(listino.recordlistino, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpPost]
        public JsonResult leggi_movimenti(RecordListinoModel item, string da_data, string a_data)
        {
            DateTime data_da = new DateTime(DateTime.Now.Year - 1, 1, 1);
            DateTime data_a = new DateTime(DateTime.Now.Year, 12, 31);

            if (!string.IsNullOrEmpty(da_data))
                data_da = Convert.ToDateTime(da_data);

            if (!string.IsNullOrEmpty(a_data))
            {
                data_a = Convert.ToDateTime(a_data);
                data_a = data_a.AddMonths(1).AddDays(-1);
            }
                

            ListaMovArticoloModel mov_articolo = new ListaMovArticoloModel();
            mov_articolo.id_codice_art = item.id_codice_art;
            mov_articolo.select(con, data_da,data_a);

            return Json(mov_articolo, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
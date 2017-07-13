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
        private const int REC_X_PAGINA = 15;

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

            CategorieStruttura categorie = new CategorieStruttura();
            categorie.select(con);

            var jsonResult = Json( categorie.categorie , JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }


        [HttpGet]
        public JsonResult GetPaginatore(string query, string cod_cat_merc)
        {
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
                    cmd.CommandText += " and id_categoria_merc like ('" + cod_cat_merc + "')";
                }
                cmd.Parameters.AddWithValue("query", "%" + query + "%");
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

            Listino listino = new Listino();
            listino.select(con, query, cod_cat_merc, page_number, REC_X_PAGINA);



            var jsonResult = Json(listino.recordlistino, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpPost]
        public JsonResult saveListino(RecordListino item)
        {
            con.Open();

            item.scriviPrezzi(con);

            con.Close();
            return Json(new { ack="OK"}, JsonRequestBehavior.AllowGet);
        }



        protected override void Dispose(bool disposing)
        {           
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
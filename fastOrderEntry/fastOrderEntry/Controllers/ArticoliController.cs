using fastOrderEntry.Helpers;
using fastOrderEntry.Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace fastOrderEntry.Controllers
{
    public class ArticoliController : Controller
    {
        private const int REC_X_PAGINA = 15;

        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public ArticoliController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        // GET: ListiniArticolo
        public ActionResult Index()
        {
            ViewBag.page = "anagrafiche";
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
                cmd.Parameters.AddWithValue("query",  query + "%");
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
        public JsonResult saveListino(RecordListinoModel item)
        {
            con.Open();

            item.scriviPrezzi(con);

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult leggi_articolo(RecordListinoModel item)
        {
            AnaArticoloModel articolo = new AnaArticoloModel();
            articolo.id_codice_art_rif = item.id_codice_art;
            articolo.select();

            return Json(articolo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult copia_articolo(AnaArticoloModel articolo)
        {
            using (PetLineContext db = new PetLineContext())
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();

                string dati_copia = JsonConvert.SerializeObject(articolo);

                var settings = (from item in db.impostazioni
                                select item).First();

                values["op"] = "copia_articolo";
                values["dati_copia"] = dati_copia;
                values["private_key"] = settings.private_key;

                try
                {
                    var response = client.UploadValues(settings.jerp_url + "/zwebServ/sync.jsp", values);
                    var responseString = Encoding.Default.GetString(response);
                }
                catch (WebException e)
                {
                    var messaggio = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                    return Json(new { ack = "KO", messaggio = messaggio }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
using fastOrderEntry.Helpers;
using fastOrderEntry.Models;
using Npgsql;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fastOrderEntry.Controllers
{
    public class ListiniClienteController : Controller
    {
        private const int REC_X_PAGINA = 15;
        private NpgsqlConnection con = null;

        public ListiniClienteController()
        {
            con = DbUtils.GetDefaultConnection();
        }        

        public ActionResult Index()
        {
            ViewBag.page = "anagrafiche"; 
            return View();
        }

        public ActionResult stampa(string query, string cod_cat_merc_sel, string id_cliente)
        {
            con.Open();

            ListinoCliente listino = new ListinoCliente();
            listino.select(con, query, cod_cat_merc_sel, id_cliente);
            con.Close();

            var wk = listino.recordlistino
                .Select(x =>
                {
                    return new ReportListinoModel()
                    {
                        articolo = x.id_codice_art,
                        descrizione = x.descrizione,
                        prezzo_di_vendita = x.prezzo_vendita,
                        giacenza = x.giacenza,
                        sconto_1 = x.sconto_1,
                        sconto_2 = x.sconto_2,
                        sconto_3 = x.sconto_3,
                        prezzo_netto = x.prezzo_vendita * (100 - x.sconto_1) / 100 * (100 - x.sconto_2) / 100 * (100 - x.sconto_3) / 100 * (100 - x.sconto_agente) / 100,
                        iva = x.id_iva,
                        prezzo_di_acquisto = x.prezzo_acquisto
                    };
                })
                .OrderBy(x => x.descrizione)
                .ToList();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.Cells[1, 1].LoadFromCollection(wk, true);

            //for (int c = 3; c <= 8; c++)
            //{
            //    workSheet.Column(c).Style.Numberformat.Format = "#,##0";
            //}

            //workSheet.Column(10).Style.Numberformat.Format = "#,##0";

            workSheet.Cells["A1:J1"].Style.Font.Bold = true;
            workSheet.Cells.AutoFitColumns();

            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=listino_cliente_" + id_cliente.TrimStart('0') + "_" + cod_cat_merc_sel + "_" + query + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

            return View();
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
        public JsonResult GetConenutoPagina(string query, string cod_cat_merc, string id_cliente, int page_number)
        {
            con.Open();

            ListinoCliente listino = new ListinoCliente();
            listino.select(con, query, cod_cat_merc, id_cliente, page_number, REC_X_PAGINA);



            var jsonResult = Json(listino.recordlistino, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        [HttpPost]
        public JsonResult saveListino(RecordListinoModel item)
        {
            con.Open();

            item.scriviPrezziCliente(con);

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult copia_prezzo(string query, string cod_cat_merc, string id_cliente, decimal prezzo_massivo = 0)
        {
            con.Open();

            ListinoModel listino = new ListinoModel();
            listino.update_massivo_prezzo_vendita_cliente(con, prezzo_massivo, query, cod_cat_merc, id_cliente);

            //TODO: copia listino

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult copia_sconto1(string query, string cod_cat_merc, string id_cliente, decimal sconto_massivo = 0)
        {
            con.Open();

            ListinoModel listino = new ListinoModel();
            listino.update_massivo_sconto_cliente(con, "SC01", sconto_massivo, query, cod_cat_merc, id_cliente);

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult copia_sconto2(string query, string cod_cat_merc, string id_cliente, decimal sconto_massivo = 0)
        {
            con.Open();

            ListinoModel listino = new ListinoModel();
            listino.update_massivo_sconto_cliente(con, "SC02", sconto_massivo, query, cod_cat_merc, id_cliente);

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult copia_sconto3(string query, string cod_cat_merc, string id_cliente, decimal sconto_massivo = 0)
        {
            con.Open();

            ListinoModel listino = new ListinoModel();
            listino.update_massivo_sconto_cliente(con, "SC03", sconto_massivo, query, cod_cat_merc, id_cliente);

            con.Close();
            return Json(new { ack = "OK" }, JsonRequestBehavior.AllowGet);
        }
    }
}
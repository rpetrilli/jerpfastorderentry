using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebCore.fw
{
    /// <summary>
    /// Controller per la paginazione
    /// </summary>
    /// <typeparam name="F">Oggetto Filtri per i filtri della maschera di selezione</typeparam>
    /// <typeparam name="O">DBObject vero e proprio</typeparam>
    /// <typeparam name="R">Record id riga della pagina di visualizzazione</typeparam>
    public abstract class TabCotroller<F, O, R> : Controller
        where F: Filters, new()
        where O: DBObject, new()
    {
        private const int REC_X_PAGINA = 25;
        private NpgsqlConnection con = null;

        protected abstract NpgsqlConnection getConnection();

        public TabCotroller()
        {
            con = getConnection();
        }

        public string getScopeVars()
        {
            return new F().toScopeVariables();
        }

        /// <summary>
        /// Metodo usato per inizializzare il paginatore il client comunica i parametri di filtro impostati dall'utente
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPaginatore(F filters)
        {
            O dbObject = new O();
            int cnt = 0;
            con.Open();
            cnt = getCount(con, filters);
            con.Close();

            var jsonResult = Json(new { rec_number = cnt, rec_x_pagina = REC_X_PAGINA, pag_number = Math.Ceiling(1.0 * cnt / REC_X_PAGINA) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        /// <summary>
        /// Usato per restituire l'elenco di oggetti prensentati sulla lista nella pagina
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetConenutoPagina(F filters)
        {
            con.Open();
            List<R> page = loadPage(con, filters.page_number, REC_X_PAGINA, filters);
            con.Close();
            
            var jsonResult = Json(page, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPut]
        public JsonResult update(O obj)
        {
            con.Open();
            NpgsqlTransaction transaction = con.BeginTransaction();
            try
            {
                obj.update(con);
                transaction.Commit();
                obj.db_obj_ack = "OK";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                obj.db_obj_ack = "KO";
                obj.db_obj_message = ex.Message;
            }
            con.Close();

            var jsonResult
                = Json(obj, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        public JsonResult insert(O obj)
        {

            con.Open();
            NpgsqlTransaction transaction = con.BeginTransaction();
            try
            {
                obj.insert(con);
                transaction.Commit();
                obj.db_obj_ack = "OK";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                obj.db_obj_ack = "KO";
                obj.db_obj_message = ex.Message;
            }
            con.Close();
            
            var jsonResult
                = Json(obj, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        [HttpPost]
        public JsonResult delete(O obj)
        {
            con.Open();
            NpgsqlTransaction transaction = con.BeginTransaction();
            try
            {
                obj.delete(con);
                transaction.Commit();
                obj.db_obj_ack = "OK";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                obj.db_obj_ack = "KO";
                obj.db_obj_message = ex.Message;
            }
            con.Close();

            var jsonResult
                = Json(obj, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        [HttpGet]
        public JsonResult select(O obj)
        {
            con.Open();
            obj.select(con);
            con.Close();

            var jsonResult
                = Json(obj, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        abstract public int getCount(NpgsqlConnection con, F filters);

        abstract public List<R> loadPage(NpgsqlConnection con, int first, int pageSize, F filters);


        protected string getLimStr(int pag_corrente, int nr_reg_x_pagina)
        {
            return " LIMIT " + nr_reg_x_pagina + " OFFSET " + pag_corrente * nr_reg_x_pagina;
        }


        public new ViewResult View()
        {
            ViewResult view = base.View();

            ViewBag.scope_var = new F().toScopeVariables();
            return view;
        }

    }
}
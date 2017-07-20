using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebCore.fw
{
    public abstract class TabCotroller<F, O> : Controller
        where F: Filters, new()
        where O: DBObject, new()
    {
        private const int REC_X_PAGINA = 15;
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
            cnt = dbObject.getCount(con, filters);
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
            O dbObject = new O();
            List<O> page = new List<O>();
            con.Open();
            foreach (O obj in dbObject.loadPage(con, filters.page_number, REC_X_PAGINA, filters))
            {
                page.Add(obj);
            }
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
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            con.Close();
            
            var jsonResult
                = Json(obj, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }


        [HttpDelete]
        public JsonResult delete(O obj)
        {
            obj.delete(con);
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


        public new ViewResult View()
        {
            ViewResult view = base.View();
            ViewBag.scope_var = new F().toScopeVariables();
            return view;
        }

    }
}
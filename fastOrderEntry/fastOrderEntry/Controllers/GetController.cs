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
    public class GetController : Controller
    {
        private NpgsqlConnection con = null;

        public GetController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        public JsonResult GetClienti(string query)
        {
            con.Open();
            ClientiStrutturaModel clienti = new ClientiStrutturaModel();
            clienti.select(con,query);

            var jsonResult = Json(clienti.rs, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        public JsonResult GetAgenti(string query)
        {
            con.Open();
            AgentiStrutturaModel agenti = new AgentiStrutturaModel();
            agenti.select(con, query);

            var jsonResult = Json(agenti.agenti, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        public JsonResult GetVettori()
        {
            con.Open();
            VettoreStrutturaModel vettori = new VettoreStrutturaModel();
            vettori.select(con);

            var jsonResult = Json(vettori.vettori, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }
    }
}
using fastOrderEntry.Helpers;
using fastOrderEntry.Models;
using Newtonsoft.Json;
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

        public ContentResult GetVettori()
        {
            con.Open();
            VettoreStrutturaModel vettori = new VettoreStrutturaModel();
            vettori.select(con);

            var jsonResult = Json(vettori.rs, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();

            string output = JsonConvert.SerializeObject(vettori.rs);
            return Content(output);
        }

        public JsonResult GetArticoli(string id_cliente, string query)
        {
            con.Open();


            ArticoloStrutturaModel articoli = new ArticoloStrutturaModel();
            articoli.select(con, id_cliente, query);

            var jsonResult = Json(articoli.rs, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }

        public ActionResult GetCondPag()
        {
            List<CondizionePag> model = new List<CondizionePag>();
            using (PetLineContext db = new PetLineContext())
            {
                model = db.condizionePag.ToList();
            }

            string output = JsonConvert.SerializeObject(model);
            return Content(output);

        }


        public ActionResult GetAgentiOrdine()
        {
            List<Agenti> model = new List<Agenti>();
            using (PetLineContext db = new PetLineContext())
            {
                model = db.agenti.ToList();
            }

            string output = JsonConvert.SerializeObject(model);
            return Content(output);

        }        

    }
}
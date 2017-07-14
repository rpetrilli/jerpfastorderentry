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

        public JsonResult GetClienti()
        {
            con.Open();
            ClientiStruttura clienti = new ClientiStruttura();
            clienti.select(con);

            var jsonResult = Json(clienti.clienti, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            con.Close();
            return jsonResult;
        }
    }
}
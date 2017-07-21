using fastOrderEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.fw;
using Npgsql;
using fastOrderEntry.Helpers;

namespace fastOrderEntry.Controllers
{
    public class OrdiniVenditaController : TabCotroller<OrdiniFilters, OrdineVenditaModel>
    {
        // GET: OrdiniVendita
        public ActionResult Index()
        {
            
            return View();
        }

        protected override NpgsqlConnection getConnection()
        {
            return DbUtils.GetDefaultConnection();   
        }
    }


    public class OrdiniFilters : Filters
    {
        public string id_cliente { get; set; }
        public string id_agente { get; set; }

        public DateTime? da_data { get; set; }
        public DateTime? al_data { get; set; }

        public string id_ordine_da { get; set; }
        public string id_ordine_al { get; set; }

        protected override void buildWhere()
        {
            addStringExactValue("vo_ordine.id_cliente", id_cliente);
            //addStringExactValue("va_clienti.id_cliente", id_agente);
            addDateRange("vo_ordine.data_ordine", da_data, al_data);
            addStringRange("vo_ordine.id_ordine", id_ordine_da, id_ordine_al);
            
        }
    }
}
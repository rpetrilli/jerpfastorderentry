using fastOrderEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.fw;
using Npgsql;
using fastOrderEntry.Helpers;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace fastOrderEntry.Controllers
{
    public class OrdiniVenditaController : TabCotroller<OrdiniFilters, OrdineVenditaModel, RigaElenco>
    {
        private PetLineContext db = new PetLineContext();

        // GET: OrdiniVendita
        public ActionResult Index()
        {
            ViewBag.page = "vendite";

            string username;
            string first_name;
            string last_name;

            //using (PetLineContext db = new PetLineContext())
            //{

            //}

            var array = User.Identity.Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            first_name = array[0];
            if (array.Count() > 1)
            {
                last_name = array.Count() > 1 ? array[1] : null;
                username = db.utenti.FirstOrDefault(x => x.first_name == first_name & x.last_name == last_name).user_name;
            }
            else
            {
                username = db.utenti.FirstOrDefault(x => x.first_name == first_name).user_name;
            }

            ViewBag.username = username;
            return View();
        }

        public override int getCount(NpgsqlConnection con, OrdiniFilters filters)
        {    
            int count = 0;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT \r\n" +
                "      count(*) as cnt \r\n" +
                "from vo_ordini " +
                /*"left join vo_ordini_provv_testata \r\n";
                "   on  vo_ordini_provv_testata.id_divisione = vo_ordini.id_divisione \r\n" +
                "   and  vo_ordini_provv_testata.esercizio = vo_ordini.esercizio \r\n" +
                "   and  vo_ordini_provv_testata.id_ordine = vo_ordini.id_ordine \r\n" +*/
                filters.toWhereConditions(filters.ordine_chiuso);

                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = Convert.ToInt32(reader["cnt"]);
                    }
                }
            }
            return count;
        }

        public override List<RigaElenco> loadPage(NpgsqlConnection con, int first, int pageSize, OrdiniFilters filters)
        {
            List<RigaElenco> list = new List<RigaElenco>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT \r\n" +
                    "      vo_ordini.*, \r\n" +
                    "       (select ragione_sociale from va_clienti where id_cliente = vo_ordini.id_cliente ) as ragione_sociale, \r\n" +
                    "       (select id_gc_cliente_id from va_clienti where id_cliente = vo_ordini.id_cliente ) as id_gc_cliente_id, \r\n" +
                    "       vo_ordini.zpet_id_agente as id_agente, \r\n" +
                    "       va_agenti.ragione_sociale as ragione_sociale_agente, \r\n" +
                    "       va_agenti.id_tipo_agente as id_tipo_agente, \r\n" +
                    "       (select 'esercizio=' || esercizio || '&id_consegna=' || id_consegna from vo_consegne_righe where id_divisione= '1' and esercizio_ordine = vo_ordini.esercizio and id_ordine_vend = vo_ordini.id_ordine limit 1) as link_consegna, \r\n" +
                    "       (select 'esercizio=' || esercizio || '&id_fattura=' || id_fattura from vo_fatture_righe where id_societa='1' and esercizio_ordine = vo_ordini.esercizio and id_ordine_vend = vo_ordini.id_ordine limit 1)  as link_fattura \r\n" +
                    "from vo_ordini \r\n" +
                    "left join va_agenti on \r\n" +
                    "   va_agenti.id_agente = vo_ordini.zpet_id_agente \r\n" +
                    filters.toWhereConditions(filters.ordine_chiuso) +
                    "order by esercizio desc, id_ordine desc \r\n"+
                    this.getLimStr(first, pageSize);

                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RigaElenco item = new RigaElenco();
                        item.id_cliente = Convert.ToString(reader["id_cliente"]);
                        item.ragione_sociale = Convert.ToString(reader["ragione_sociale"]);
                        item.id_agente = Convert.ToString(reader["id_agente"]);
                        item.ragione_sociale_agente = Convert.ToString(reader["ragione_sociale_agente"]);  
                        item.id_ordine = Convert.ToString(reader["id_ordine"]);
                        item.data_ordine = Convert.ToDateTime(reader["data_ordine"]).ToString("yyyy-MM-dd");
                        item.esercizio = Convert.ToInt32(reader["esercizio"]);
                        item.totale_doc = Convert.ToDecimal(reader["totale_doc"]);
                        item.ordine_chiuso = Convert.ToBoolean(reader["ordine_chiuso"]) || Convert.ToBoolean(reader["chiudi_forzatamente"]); ;
                        item.id_tipo_agente = Convert.ToString(reader["id_tipo_agente"]);
                        item.id_gc_cliente_id = Convert.ToString(reader["id_gc_cliente_id"]);
                        item.link_consegna = Convert.ToString(reader["link_consegna"]);
                        item.link_fattura = Convert.ToString(reader["link_fattura"]);
                        item.stampato = string.IsNullOrEmpty(reader["stampato"].ToString()) ? false : Convert.ToBoolean(reader["stampato"]);
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        [HttpPost]
        public ContentResult DdtFattura(OrdineVenditaModel model)
        {
            //using (PetLineContext db = new PetLineContext())
            //{

            //}
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();

                string ordine = JsonConvert.SerializeObject(model);

                var settings = (from item in db.impostazioni
                                select item).First();

                if (model.tipo == "ddt")
                {
                    values["op"] = "ordine_to_consegna";
                }
                else if (model.tipo == "fat")
                {
                    values["op"] = "ordine_to_fattura";
                }

                values["ordine"] = ordine;
                values["private_key"] = settings.private_key;

                try
                {
                    var response = client.UploadValues(settings.jerp_url + "/zwebServ/sync.jsp", values);
                    var responseString = Encoding.Default.GetString(response);

                    JObject obj = JObject.Parse(responseString);
                    obj.Add("ack", "OK");

                    return Content(obj.ToString(), "application/json");

                }
                catch (WebException e)
                {
                    var messaggio = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                    JObject obj = new JObject();
                    obj.Add("ack", "KO");
                    obj.Add("messaggio", messaggio);
                    return Content(obj.ToString(), "application/json");
                }
            }

        }

        [HttpPost]
        public ContentResult MassivoDdt(List<OrdineVenditaModel> model)
        {
            JObject obj = setMassivo(model, "ordini_to_consegna");
            return Content(obj.ToString(), "application/json");
        }

        [HttpPost]
        public ContentResult MassivoFat(List<OrdineVenditaModel> model)
        {
            JObject obj = setMassivo(model, "ordini_to_fattura");
            return Content(obj.ToString(), "application/json");
        }

        private JObject setMassivo(List<OrdineVenditaModel> model, string op)
        {            
            JObject obj = new JObject();

            //using ()
            //{

            //}

            using (var client = new WebClient())
            {

                if (model.Count(x => x.massivo == true) > 0)
                {
                    // controllo se l'agente è univoco altrimenti mando a cacare
                    //

                    string id_agente = model.FirstOrDefault(x => x.massivo == true).id_agente;
                    string id_cliente = model.FirstOrDefault(x => x.massivo == true).id_cliente;
                    if (model.Count(x => (x.id_agente != id_agente & x.massivo == true) | (x.id_cliente != id_cliente & x.massivo == true)) == 0)
                    {
                        var values = new NameValueCollection();
                        string ordini = JsonConvert.SerializeObject(model.Where(x => x.massivo == true));
                        var settings = (from item in db.impostazioni
                                        select item).First();

                        values["op"] = op;
                        values["ordini"] = ordini;
                        values["private_key"] = settings.private_key;

                        try
                        {
                            var response = client.UploadValues(settings.jerp_url + "/zwebServ/sync.jsp", values);
                            var responseString = Encoding.Default.GetString(response);
                            obj = JObject.Parse(responseString);
                            obj.Add("ack", "OK");

                        }

                        catch (WebException e)
                        {
                            var messaggio = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                            obj.Add("ack", "KO");
                            obj.Add("messaggio", messaggio);
                        }
                    }
                    else
                    {
                        obj.Add("ack", "KO");
                        obj.Add("messaggio", "l'agente o il cliente deve essere univoco");
                    }
                }

                else
                {
                    obj.Add("ack", "KO");
                    obj.Add("messaggio", "devi selzionare almeno un ordine");
                }
            }


            return obj;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
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

        public string da_data { get; set; }
        public string al_data { get; set; }

        public string id_ordine_da { get; set; }
        public string id_ordine_al { get; set; }

        public string ordine_chiuso { get; set; }

        protected override void buildWhere()
        {   
            addStringExactValue("vo_ordini.id_cliente", id_cliente);
            addStringExactValue("vo_ordini.zpet_id_agente", id_agente);
            addDateRange("vo_ordini.data_ordine", strToDateTime(da_data), strToDateTime(al_data));
            addStringRange("vo_ordini.id_ordine", strWithZeros(id_ordine_da), strWithZeros(id_ordine_al));
            
        }
    }

    public class RigaElenco
    {
        public int esercizio { get; set; }
        public string id_ordine { get; set; }
        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        public string id_agente { get; set; }
        public string id_tipo_agente { get; set; }
        public string ragione_sociale_agente { get; set; }
        public string data_ordine { get; set; }
        public decimal totale_doc { get; set; }
        public bool ordine_chiuso { get; set; }
        public string id_gc_cliente_id { get; set; }
        public string link_consegna { get; set; }
        public string link_fattura { get; set; }
        public bool stampato { get; set; }

    }
}
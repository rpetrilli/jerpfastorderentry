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
    public class OrdiniVenditaController : TabCotroller<OrdiniFilters, OrdineVenditaModel, RigaElenco>
    {
        // GET: OrdiniVendita
        public ActionResult Index()
        {

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
                    "from vo_ordini ";
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
                    "      * \r\n" +
                    "from vo_ordini \r\n" + this.getLimStr(first, pageSize);

                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RigaElenco item = new RigaElenco();
                        item.id_cliente = Convert.ToString(reader["id_cliente"]);
                        item.id_ordine = Convert.ToString(reader["id_ordine"]);
                        item.data_ordine = Convert.ToDateTime(reader["data_ordine"]).ToString("yyyyMMdd");
                        item.esercizio = Convert.ToInt32(reader["esercizio"]);
                        list.Add(item);
                    }
                }
            }

            return list;
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

    public class RigaElenco
    {
        public int esercizio { get; set; }
        public string id_ordine { get; set; }
        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        public string data_ordine { get; set; }

    }
}
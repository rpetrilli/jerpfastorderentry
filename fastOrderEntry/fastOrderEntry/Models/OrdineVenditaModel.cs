using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using WebCore.fw;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace fastOrderEntry.Models
{
    public class OrdineVenditaModel : DBObject
    {
        public int esercizio { get; set; }
        public string id_ordine { get; set; }
        public DateTime data_ordine { get; set; }
        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        public string id_cond_pag { get; set; }
        public string indirizzo { get; set; }
        public string cap { get; set; }
        public string comune { get; set; }
        public string provincia { get; set; }
        public string nazioni { get; set; }


        public override void delete(NpgsqlConnection con)
        {
            using (PetLineContext db = new PetLineContext()) { 
                var settings = (from item in db.impostazioni
                                select item).First();

                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["op"] = "delete_order";
                    values["esercizio"] = esercizio.ToString();
                    values["id_ordine"] = id_ordine;
                    values["private_key"] = settings.private_key;

                    var response = client.UploadValues(settings.jerp_url, values);

                    var responseString = Encoding.Default.GetString(response);
                }

            }


        }


        public override void insert(NpgsqlConnection con)
        {
            using (PetLineContext db = new PetLineContext())
            {
                var settings = (from item in db.impostazioni
                                select item).First();

                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();

                    string ordine = JsonConvert.SerializeObject(this);

                    values["op"] = "insert_order";
                    values["ordine"] = ordine;
                    values["private_key"] = settings.private_key;

                    try
                    {
                        var response = client.UploadValues(settings.jerp_url, values);
                        var responseString = Encoding.Default.GetString(response);
                    }
                    catch (WebException e) 
                    {
                        var messaggio = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                        throw new Exception(messaggio);
                    }
                }

            }
        }

        public override void select(NpgsqlConnection con)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT \r\n" +
                    "      * \r\n" +
                    "from vo_ordini \r\n" +
                    "where id_societa = '1' and esercizio = @esercizio and id_ordine = @id_ordine";

                cmd.Parameters.AddWithValue("esercizio", esercizio);
                cmd.Parameters.AddWithValue("id_ordine", id_ordine);

                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id_cliente = Convert.ToString(reader["id_cliente"]);
                        data_ordine = Convert.ToDateTime(reader["data_ordine"]);
                        ragione_sociale = Convert.ToString(reader["id_cliente"]);

                    }
                }
            }
        }

        public override void update(NpgsqlConnection con)
        {
            using (PetLineContext db = new PetLineContext())
            {
                var settings = (from item in db.impostazioni
                                select item).First();

                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();

                    string ordine = JsonConvert.SerializeObject(this);

                    values["op"] = "update_order";
                    values["ordine"] = ordine;
                    values["private_key"] = settings.private_key;

                    var response = client.UploadValues(settings.jerp_url, values);

                    var responseString = Encoding.Default.GetString(response);
                }

            }
        }
    }

    public class OrdineRiga
    {
        public long nr_riga { get; set; }
        public string id_iva { get; set; }
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public string id_um { get; set; }
        public decimal quantita { get; set; }
        public decimal prezzo_unitario { get; set; }
        public string str_sconto { get; set; }

        public decimal qta_ordinata { get; set; }
        public decimal qta_in_consegna { get; set; }
        public decimal sconto_1 { get; set; }
        public decimal sconto_2 { get; set; }
        public decimal sconto_3 { get; set; }
    }

}
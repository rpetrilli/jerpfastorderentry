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
        public string id_vettore { get; set; }
        public IList<OrdineRiga> righe { get; set; }
        public string name { get; set; }
        public string tipo { get; set; }
        public string id_gc_cliente_id { get; set; }
        public bool massivo { get; set; }
        public bool ordine_chiuso { get; set; }
        public string id_agente { get; set; }
        public string note_magazzino { get; set; }
        public string noteordine { get; set; }
        public int colli { get; set; }
        public string username { get; set; }
        public decimal sconto_cassa { get; set; }

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
                    values["username"] = username;

                    var response = client.UploadValues(settings.jerp_url + "/zwebServ/sync.jsp", values);

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
                    values["username"] = username;
                    values["private_key"] = settings.private_key;

                    try
                    {
                        var response = client.UploadValues(settings.jerp_url + "/zwebServ/sync.jsp", values);
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
                    "      vo_ordini.*, \r\n" +
                    "       (select id_gc_cliente_id from va_clienti where id_cliente = vo_ordini.id_cliente ) as id_gc_cliente_id \r\n" +
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
                        ragione_sociale = Convert.ToString(reader["ragione_sociale"]);
                        name = Convert.ToString(reader["ragione_sociale"]);
                        indirizzo = Convert.ToString(reader["indirizzo"]);
                        cap = Convert.ToString(reader["cap"]);
                        nazioni = Convert.ToString(reader["nazioni"]);
                        id_vettore = Convert.ToString(reader["zpet_id_vettore"]);
                        id_cond_pag = Convert.ToString(reader["id_cond_pag"]);
                        id_gc_cliente_id = Convert.ToString(reader["id_gc_cliente_id"]);
                        ordine_chiuso = Convert.ToBoolean(reader["ordine_chiuso"]);
                        id_agente = Convert.ToString(reader["zpet_id_agente"]);
                        note_magazzino = Convert.ToString(reader["zpet_note_magazzino"]);
                        noteordine = Convert.ToString(reader["nota"]);
                        colli = !string.IsNullOrEmpty(Convert.ToString(reader["zpet_colli"])) ? Convert.ToInt32(reader["zpet_colli"]) : 1;
                    }
                }
            }

            righe = new List<OrdineRiga>();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT \r\n" +
                    "      * \r\n" +
                    "from vo_ordini_righe \r\n" +
                    "left join ma_articoli_soc " +
                    "   on ma_articoli_soc.id_societa = '1' " +
                    "   and ma_articoli_soc.id_codice_art = vo_ordini_righe.id_codice_art \r\n" +
                    "left join ca_iva \r\n" +
                    "   on ca_iva.id_societa = '1' \r\n" +
                    "   and ca_iva.id_iva = ma_articoli_soc.id_iva \r\n" +
                    "where vo_ordini_righe.id_societa = '1' \r\n" +
                    "  and vo_ordini_righe.esercizio = @esercizio \r\n" +
                    "  and vo_ordini_righe.id_ordine = @id_ordine \r\n" +
                    "order by nr_riga";

                cmd.Parameters.AddWithValue("esercizio", esercizio);
                cmd.Parameters.AddWithValue("id_ordine", id_ordine);

                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrdineRiga item = new OrdineRiga();
                        item.nr_riga = Convert.ToInt32(reader["nr_riga"]);
                        item.id_iva = Convert.ToString(reader["id_iva"]);
                        item.id_codice_art = Convert.ToString(reader["id_codice_art"]);
                        item.descrizione = Convert.ToString(reader["descrizione"]);
                        item.id_um = Convert.ToString(reader["id_um"]);
                        item.quantita = Convert.ToDecimal(reader["quantita"]);

                        item.prezzo_vendita = Convert.ToDecimal(reader["prezzo_unitario"]);
                        try { item.sconto_1 = Convert.ToDecimal(reader["zpet_sconto_1"]); } catch { }
                        try { item.sconto_2 = Convert.ToDecimal(reader["zpet_sconto_2"]); } catch { }
                        try { item.sconto_3 = Convert.ToDecimal(reader["zpet_sconto_3"]); } catch { }
                        try { item.sconto_agente = Convert.ToDecimal(reader["zpet_sconto_agente"]); } catch { }
                        try { item.qta_ordinata = Convert.ToDecimal(reader["zpet_qta_ordinata"]); } catch { }
                        try{ item.qta_in_consegna = Convert.ToDecimal(reader["zpet_qta_in_consegna"]); } catch { }
                        try { item.peso_lordo = Convert.ToDecimal(reader["peso_lordo"]); } catch { }
                        try { item.aliquota = Convert.ToDecimal(reader["aliquota"]); } catch { }

                        righe.Add(item);


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
                    values["username"] = username;
                    values["private_key"] = settings.private_key;
                    

                    var response = client.UploadValues(settings.jerp_url + "/zwebServ/sync.jsp", values);

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
        public decimal prezzo_vendita { get; set; }
        public string str_sconto { get; set; }

        public decimal qta_ordinata { get; set; } = 0;
        public decimal qta_in_consegna { get; set; } = 0;
        public decimal sconto_1 { get; set; } = 0;
        public decimal sconto_2 { get; set; } = 0;
        public decimal sconto_3 { get; set; } = 0;
        public decimal sconto_agente { get; set; } = 0;
        public decimal peso_lordo { get; set; } = 0;
        public decimal aliquota { get; set; } = 0;
        
    }

}
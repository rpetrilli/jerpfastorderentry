using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using WebCore.fw;

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
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;

                cmd.CommandText = "delete from vo_ordini_righe_provv " +
                "where id_divisione = '1' and esercizio = @esercizio and id_ordine = @id_ordine";
                cmd.Parameters.AddWithValue("esercizio", esercizio);
                cmd.Parameters.AddWithValue("id_ordine", id_ordine);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;

                cmd.CommandText = "delete from vo_ordini_righe_provv " +
                "where id_divisione = '1' and esercizio = @esercizio and id_ordine = @id_ordine";
                cmd.Parameters.AddWithValue("esercizio", esercizio);
                cmd.Parameters.AddWithValue("id_ordine", id_ordine);
                cmd.ExecuteNonQuery();
            }
        }


        public override void insert(NpgsqlConnection con)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
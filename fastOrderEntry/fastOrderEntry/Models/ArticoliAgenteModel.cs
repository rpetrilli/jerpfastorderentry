using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class ArticoliAgenteModel
    {
        public ArticoliAgenteModel()
        {
            this.recordArticoli = new List<RecordArticoliAgenteModel>();
        }

        public virtual IList<RecordArticoliAgenteModel> recordArticoli { get; set; }

        internal void select(NpgsqlConnection con, string query, string cod_cat_merc = "", string id_agente = "", int pagina = 0, int REC_X_PAGINA = 0)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT * \r\n" +
                    " from ma_articoli_soc \r\n" +
                    "where id_societa = '1' \r\n";
                if (!string.IsNullOrEmpty(query))
                {
                    cmd.CommandText += "  and (upper(id_codice_art) LIKE( @query) or upper(descrizione) like( @query ) ) \r\n";
                }
                if (!string.IsNullOrEmpty(cod_cat_merc))
                {
                    cmd.CommandText += " and (id_categoria_merc like ('" + cod_cat_merc + "-%') or id_categoria_merc ='" + cod_cat_merc + "')";
                }

                if (REC_X_PAGINA > 0)
                {
                    cmd.CommandText += "limit " + REC_X_PAGINA + " offset " + (pagina * REC_X_PAGINA);
                }
                if (!string.IsNullOrEmpty(query))
                {
                    cmd.Parameters.AddWithValue("query", query.ToUpper() + "%");
                }
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecordArticoliAgenteModel r = new RecordArticoliAgenteModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordArticoli.Add(r);
                    }
                }

                foreach (RecordArticoliAgenteModel r in recordArticoli)
                {
                    r.leggiValori(con, id_agente);
                }

            }
        }

    }

    public class RecordArticoliAgenteModel
    {
        public string id_societa { get; set; }
        public string id_agente { get; set; }
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public bool visibile { get; set; }

        internal void leggiValori(NpgsqlConnection con, string id_agente)
        {
            this.id_agente = id_agente;
            visibile = leggiVisibile(con, id_agente);
        }

        internal void ScriviVisibile(NpgsqlConnection con)
        {
            updateArticoloAgente(con, visibile, "1");
        }

        private void updateArticoloAgente(NpgsqlConnection con, bool visibile, string id_societa)
        {
            int cnt = 0;
            if (!visibile)
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "update zpet_agente_articoli set visibile = @visibile " +
                        "where id_societa = '1' " +
                        "  and id_codice_art = @id_codice_art " +
                        "  and id_agente = @id_agente ";
                    cmd.Parameters.AddWithValue("visibile", visibile);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_agente", id_agente);
                    cnt = cmd.ExecuteNonQuery();
                }
                if (cnt == 0)
                {
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO zpet_agente_articoli( \r\n" +
                            "            id_societa, visibile, id_codice_art, \r\n" +
                            "             id_agente) \r\n" +
                            "    VALUES('1', @visibile, @id_codice_art, \r\n" +
                            "            @id_agente)";
                        cmd.Parameters.AddWithValue("visibile", visibile);
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("id_agente", id_agente);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM zpet_agente_articoli \r\n" +
                        "   where id_societa = '1'  \r\n" +
                        "     and id_codice_art =  @id_codice_art  \r\n" +
                        "     and id_agente =  @id_agente ";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_agente", id_agente);
                    cnt = cmd.ExecuteNonQuery();
                }
            }
        }

        private bool leggiVisibile(NpgsqlConnection con, string id_agente)
        {
            bool model = true;
            if (string.IsNullOrEmpty(id_agente))
                return model;

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = @"SELECT * FROM zpet_agente_articoli
                                        WHERE id_societa= '1' AND id_agente= @id_agente
                                        AND id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_agente", id_agente);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model = reader.GetBoolean(reader.GetOrdinal("visibile"));
                    }
                }
            }
            return model;
        }
        
    }
}
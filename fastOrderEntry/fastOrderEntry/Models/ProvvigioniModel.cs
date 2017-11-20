using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class ProvvigioniModel
    {
        public ProvvigioniModel()
        {
            this.recordprovvigione = new List<RecordProvvigioneModel>();
        }               

        public virtual IList<RecordProvvigioneModel> recordprovvigione { get; set; }

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
                        RecordProvvigioneModel r = new RecordProvvigioneModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordprovvigione.Add(r);
                    }
                }

                foreach (RecordProvvigioneModel r in recordprovvigione)
                {
                    r.leggiValori(con, id_agente);
                }

            }
        }

        internal void update_massivo_provvigione(NpgsqlConnection con, decimal valore_massivo, string query, string cod_cat_merc, string id_agente)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT * from ma_articoli_soc \r\n" +
                    "where id_societa = '1' \r\n";
                if (!string.IsNullOrEmpty(query))
                {
                    cmd.CommandText += "  and (upper(id_codice_art) LIKE( @query) or upper(descrizione) like( @query ) ) \r\n";
                }
                if (!string.IsNullOrEmpty(cod_cat_merc))
                {
                    cmd.CommandText += " and (id_categoria_merc like ('" + cod_cat_merc + "-%') or id_categoria_merc ='" + cod_cat_merc + "')";
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
                        RecordProvvigioneModel r = new RecordProvvigioneModel();
                        r.id_agente = id_agente;
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordprovvigione.Add(r);
                    }
                }

                foreach (RecordProvvigioneModel r in recordprovvigione)
                {
                    r.updateProvvigioneAgente(con, valore_massivo, "1");
                }
            }

        }
    }

    public class RecordProvvigioneModel
    {
        public string id_societa { get; set; }
        public string id_agente { get; set; }
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public decimal val_provvigione { get; set; }

        internal void leggiValori(NpgsqlConnection con, string id_agente)
        {
            this.id_agente = id_agente;
            val_provvigione = leggiProvvigione(con, id_agente);
        }

        private decimal leggiProvvigione(NpgsqlConnection con, string id_agente)
        {
            decimal model = 0;
            if (string.IsNullOrEmpty(id_agente))
                return model;

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = @"SELECT * FROM va_provvigioni_agente_articolo 
                                        WHERE id_societa= '1' AND id_agente= @id_agente
                                        AND id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_agente", id_agente);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model = reader.GetDecimal(reader.GetOrdinal("val_provvigione"));
                    }
                }
            }
            return model;
        }

        internal void ScriviProvvigione(NpgsqlConnection con)
        {
            updateProvvigioneAgente(con, val_provvigione, "1");

        }

        public void updateProvvigioneAgente(NpgsqlConnection con, decimal val_provvigione, string id_societa)
        {
            int cnt = 0;
            if (val_provvigione > 0)
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "update va_provvigioni_agente_articolo set val_provvigione = @val_provvigione " +
                        "where id_societa = '1' " +
                        "  and id_codice_art = @id_codice_art " +
                        "  and id_agente = @id_agente ";
                    cmd.Parameters.AddWithValue("val_provvigione", val_provvigione);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_agente", id_agente);
                    cnt = cmd.ExecuteNonQuery();
                }
                if (cnt == 0)
                {
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO va_provvigioni_agente_articolo( \r\n" +
                            "            id_societa, val_provvigione, id_codice_art, \r\n" +
                            "             id_agente) \r\n" +
                            "    VALUES('1', @val_provvigione, @id_codice_art, \r\n" +
                            "            @id_agente)";
                        cmd.Parameters.AddWithValue("val_provvigione", val_provvigione);
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
                    cmd.CommandText = "DELETE FROM va_provvigioni_agente_articolo \r\n" +
                        "   where id_societa = '1'  \r\n" +
                        "     and id_codice_art =  @id_codice_art  \r\n" +
                        "     and id_agente =  @id_agente ";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_agente", id_agente);
                    cnt = cmd.ExecuteNonQuery();
                }
            }
        }

        //private void updateProvvigioneAgente(NpgsqlConnection con, decimal val_provvigione, string v)
        //{
        //    int cnt = 0;
        //    if (val_provvigione > 0)
        //    {
        //        using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = con;
        //            cmd.CommandText = "update va_provvigioni_agente_articolo set val_provvigione = @val_provvigione " +
        //                "where id_societa = '1' " +
        //                "  and id_codice_art = @id_codice_art " +
        //                "  and id_agente = @id_agente ";
        //            cmd.Parameters.AddWithValue("val_provvigione", val_provvigione);
        //            cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
        //            cmd.Parameters.AddWithValue("id_agente", id_agente);
        //            cnt = cmd.ExecuteNonQuery();
        //        }
        //        if (cnt == 0)
        //        {
        //            using (var cmd = new NpgsqlCommand())
        //            {

        //                cmd.Connection = con;
        //                cmd.CommandText = "INSERT INTO va_provvigioni_agente_articolo( \r\n" +
        //                    "            id_societa, val_provvigione, id_codice_art, \r\n" +
        //                    "             id_agente) \r\n" +
        //                    "    VALUES('1', @val_provvigione, @id_codice_art, \r\n" +
        //                    "            @id_agente)";
        //                cmd.Parameters.AddWithValue("val_provvigione", val_provvigione);
        //                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
        //                cmd.Parameters.AddWithValue("id_agente", id_agente);
        //                cnt = cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        using (var cmd = new NpgsqlCommand())
        //        {

        //            cmd.Connection = con;
        //            cmd.CommandText = "DELETE FROM va_provvigioni_agente_articolo \r\n" +
        //                "   where id_societa = '1'  \r\n" +
        //                "     and id_codice_art =  @id_codice_art  \r\n" +
        //                "     and id_agente =  @id_agente ";
        //            cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
        //            cmd.Parameters.AddWithValue("id_agente", id_agente);
        //            cnt = cmd.ExecuteNonQuery();
        //        }
        //    }
        //}


    }
}
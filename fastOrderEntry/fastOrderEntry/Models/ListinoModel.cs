using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class ListinoModel
    {
        public ListinoModel()
        {
            this.recordlistino = new List<RecordListinoModel>();
        }
        public virtual IList<RecordListinoModel> recordlistino { get; set; }

  
        public void select(NpgsqlConnection conn, string query, string cod_cat_merc = "",int pagina = 0, int REC_X_PAGINA = 0)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT *, \r\n" +
                    "   (select sum(stock_libero) from mg_stock_magazzino where id_divisione = '1' and id_codice_art = ma_articoli_soc.id_codice_art) as giacenza \r\n" +
                    " from ma_articoli_soc \r\n" +                     
                    "where id_societa = '1' \r\n";
                if (!string.IsNullOrEmpty(query)) {
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
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        r.giacenza = !string.IsNullOrEmpty(reader["giacenza"].ToString()) ? Convert.ToDecimal(reader["giacenza"]) : 0;
                        r.id_iva = reader["id_iva"].ToString();
                        recordlistino.Add(r);
                    }
                }
                
                foreach (RecordListinoModel r in recordlistino)
                {
                    r.leggiPrezzi(conn);
                }

            }
        }

        public void update_massivo_prezzo_vendita(NpgsqlConnection con, decimal prezzo_massivo, string query, string cod_cat_merc)
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
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }

                foreach (RecordListinoModel r in recordlistino)
                {
                    r.updatePrezzo(con, prezzo_massivo, "VA01");
                }
            }
        }

        public void update_massivo_prezzo_acquisto(NpgsqlConnection con, decimal prezzo_massivo, string query, string cod_cat_merc)
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
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }

                foreach (RecordListinoModel r in recordlistino)
                {
                    r.updatePrezzo(con, prezzo_massivo, "AC01");
                }
            }
        }

        public void update_massivo_prezzo_vendita_cliente(NpgsqlConnection con, decimal prezzo_massivo, string query, string cod_cat_merc, String id_cliente)
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
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_cliente = id_cliente;
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }

                foreach (RecordListinoModel r in recordlistino)
                {
                    r.updatePrezzoCliente(con, prezzo_massivo, "VA01");
                }
            }
        }

        public void update_massivo_sconto(NpgsqlConnection con, String id_condizione,  decimal sconto_massivo, string query, string cod_cat_merc)
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
                    cmd.Parameters.AddWithValue("query", "%" + query.ToUpper() + "%");
                }
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }

                foreach (RecordListinoModel r in recordlistino)
                {
                    r.updateSconto(con, sconto_massivo, id_condizione);
                }
            }
        }

        public void update_massivo_sconto_cliente(NpgsqlConnection con, String id_condizione, decimal sconto_massivo, string query, string cod_cat_merc, String id_cliente)
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
                    cmd.Parameters.AddWithValue("query", "%" + query.ToUpper() + "%");
                }
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_cliente = id_cliente;
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }

                foreach (RecordListinoModel r in recordlistino)
                {
                    r.updateScontoCliente(con, sconto_massivo, id_condizione);
                }
            }
        }


    }

}
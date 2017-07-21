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

  
        internal void select(NpgsqlConnection conn, string query, string cod_cat_merc = "",int pagina = 0, int REC_X_PAGINA = 0)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from ma_articoli_soc \r\n" +
                    "where id_societa = '1' \r\n";
                if (!string.IsNullOrEmpty(query)) {
                    cmd.CommandText += "  and (upper(id_codice_art) LIKE( @query) or upper(descrizione) like( @query ) ) \r\n";
                }
                if (!string.IsNullOrEmpty(cod_cat_merc))
                {
                    cmd.CommandText += " and id_categoria_merc like ('" + cod_cat_merc + "')";
                }

                    if (REC_X_PAGINA > 0)
                {
                    cmd.CommandText += "limit " + REC_X_PAGINA + " offset " + (pagina * REC_X_PAGINA); 
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
                    r.leggiPrezzi(conn);
                }

            }
        }
    }


}
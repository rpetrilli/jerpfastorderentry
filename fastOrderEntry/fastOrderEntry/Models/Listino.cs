using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class Listino
    {
        public Listino()
        {
            this.recordlistino = new List<RecordListino>();
        }
        public virtual IList<RecordListino> recordlistino { get; set; }        

        internal void leggiPrezzi(NpgsqlConnection conn, string query)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from ma_articoli_soc where id_societa = '1' and (upper(id_codice_art) LIKE( @query) or upper(descrizione) like( @query ) ) ";
                cmd.Parameters.AddWithValue("query", "%"+query+"%");
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecordListino r = new RecordListino();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }
                
                foreach (RecordListino r in recordlistino)
                {
                    r.leggiPrezzi(conn);
                }

            }
        }
    }


}
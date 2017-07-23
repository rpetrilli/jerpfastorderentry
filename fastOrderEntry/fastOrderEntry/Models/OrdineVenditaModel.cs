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


        public override void delete(NpgsqlConnection con)
        {
            throw new NotImplementedException();
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
                    }
                }
            }
        }

        public override void update(NpgsqlConnection con)
        {
            throw new NotImplementedException();
        }
    }
}
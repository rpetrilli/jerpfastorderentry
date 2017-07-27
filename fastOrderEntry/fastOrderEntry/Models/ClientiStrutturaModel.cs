using Npgsql;
using System.Collections.Generic;

namespace fastOrderEntry.Models
{
    public class ClientiStrutturaModel
    {
        public ClientiStrutturaModel()
        {
            this.rs = new List<Cliente>();
        }

        public virtual IList<Cliente> rs { get; set; }

        internal void select (NpgsqlConnection conn, string query = "")
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT \r\n" +
                    "      id_cliente,  \r\n" +
                    "      ragione_sociale, \r\n" +
                    "       * \r\n" +
                    "from va_clienti where upper(ragione_sociale) LIKE( @query) " +
                    "limit 10";

                cmd.Parameters.AddWithValue("query", "%" + query.ToUpper() + "%" );
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cliente r = new Cliente();
                        r.id = reader["id_cliente"].ToString();
                        r.name = reader["ragione_sociale"].ToString();
                        rs.Add(r);
                    }
                }
            }
        }

    }

    public class Cliente
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
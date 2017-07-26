using Npgsql;
using System.Collections.Generic;

namespace fastOrderEntry.Models
{
    public class ClientiStrutturaModel
    {
        public ClientiStrutturaModel()
        {
            this.clienti = new List<Cliente>();
        }

        public virtual IList<Cliente> clienti { get; set; }

        internal void select (NpgsqlConnection conn, string query)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT \r\n" +
                    "      id_cliente,  \r\n" +
                    "      ragione_sociale, \r\n" +
                    "       * \r\n" +
                    "from va_clienti where ragione_sociale LIKE( @query)";

                cmd.Parameters.AddWithValue("query", query);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cliente r = new Cliente();
                        r.id = reader["id_cliente"].ToString();
                        r.name = reader["ragione_sociale"].ToString();
                        clienti.Add(r);
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
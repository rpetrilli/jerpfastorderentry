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
                cmd.CommandText = "SELECT *, \r\n" +
                    "(select id_agente from va_clienti_agenti where id_cliente = va_clienti.id_cliente limit 1) as id_agente \r\n" +
                    "from va_clienti \r\n" +
                    "inner join va_clienti_soc " +
                    "   on va_clienti_soc.id_societa = '1' " +
                    "   and va_clienti_soc.id_cliente = va_clienti.id_cliente " +
                    "inner join va_clienti_div " +
                    "   on va_clienti_div.id_divisione = '1' " +
                    "   and va_clienti_div.id_cliente = va_clienti.id_cliente " +
                    "where upper(ragione_sociale) LIKE( @query) " +
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
                        r.id_cliente = reader["id_cliente"].ToString();

                        r.indirizzo = reader["indirizzo"].ToString();
                        r.cap = reader["cap"].ToString();
                        r.comune = reader["comune"].ToString();
                        r.provincia = reader["provincia"].ToString();
                        r.note = reader["note"].ToString();
                        r.id_cond_pag = reader["id_cond_pag"].ToString();
                        r.id_vettore = reader["id_vettore"].ToString();
                        r.id_agente = reader["id_agente"].ToString();
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
        public string indirizzo { get; set; }
        public string cap { get; set; }
        public string comune { get; set; }
        public string provincia { get; set; }
        public string id_cliente { get; set; }
        public string id_cond_pag { get; set; }
        public string id_vettore { get; set; }
        public string note { get; set; }
        public string id_agente { get; set; }
    }
}
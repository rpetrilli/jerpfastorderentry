using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class AgentiStrutturaModel
    {
        public AgentiStrutturaModel()
        {
            this.agenti = new List<Agenti>();
        }
        public virtual IList<Agenti> agenti { get; set; }

        internal void select(NpgsqlConnection conn, string query = "")
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT \r\n" +
                    "      id_agente,  \r\n" +
                    "      ragione_sociale, \r\n" +
                    "       * \r\n" +
                    "from va_agenti \r\n" +
                    "where upper(ragione_sociale) like ('" + query.ToUpper() + "%') \r\n" +
                    "limit 10 \r\n";
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Agenti r = new Agenti();
                        r.id = reader["id_agente"].ToString();
                        r.name = reader["ragione_sociale"].ToString();
                        agenti.Add(r);
                    }
                }
            }
        }
    }

    [Table("va_agenti", Schema = "public")]
    public class Agenti
    {
        [Key, Column("id_agente")]
        public string id { get; set; }
        [Column("ragione_sociale")]
        public string name { get; set; }
    }

    
}
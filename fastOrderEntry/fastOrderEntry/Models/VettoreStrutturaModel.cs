using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;

namespace fastOrderEntry.Models
{    
    public class VettoreStrutturaModel
    {
        public VettoreStrutturaModel()
        {
            this.rs = new List<Vettore>();
        }

        public virtual IList<Vettore> rs { get; set; }

        internal void select(NpgsqlConnection con)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT * \r\n" +
                    "from va_vettori";                
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Vettore r = new Vettore();
                        r.id_vettore = reader["id_vettore"].ToString();
                        r.ragione_sociale = reader["ragione_sociale"].ToString();                       
                        rs.Add(r);
                    }
                }
            }            
        }
    }

    public class Vettore
    {
        public string id_vettore { get; set; }
        public string ragione_sociale { get; set; }



    }
}
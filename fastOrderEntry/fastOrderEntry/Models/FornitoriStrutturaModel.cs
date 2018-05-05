using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class FonitoriStrutturaModel
    {
        public FonitoriStrutturaModel()
        {
            this.rs = new List<Fornitore>();
        }

        public virtual IList<Fornitore> rs { get; set; }

        internal void select(NpgsqlConnection con)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT * \r\n" +
                    "from aa_fornitori order by ragione_sociale";
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Fornitore r = new Fornitore();
                        r.id_fornitore = reader["id_fornitore"].ToString();
                        r.ragione_sociale = reader["ragione_sociale"].ToString();
                        rs.Add(r);
                    }
                }
            }
        }
    }

    public class Fornitore
    {
        public string id_fornitore { get; set; }
        public string ragione_sociale { get; set; }
    }
}
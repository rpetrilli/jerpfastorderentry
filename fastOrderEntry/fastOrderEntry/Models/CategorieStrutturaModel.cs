using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class CategorieStrutturaModel
    {
        public CategorieStrutturaModel()
        {
            categorie = new List<Categoria>();
        }
        public virtual IList<Categoria> categorie { get; set; }

        internal void select(NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                //cmd.CommandText = "SELECT \r\n" +
                //    "       coalesce(id_cat_padre, '') as id_cat_padre,  \r\n" +
                //    "       array_length(regexp_split_to_array(coalesce(id_cat_merc,''), '[-]'),1) as livello, \r\n" +
                //    "       * \r\n" +
                //    "from mc_cat_merc where id_societa = '1' and  \r\n " +
                //    "   array_length(regexp_split_to_array(coalesce(id_cat_merc,''), '[-]'),1) <= 2";
                //cmd.ExecuteNonQuery();

                cmd.CommandText = @"SELECT 
                               coalesce(id_cat_padre, '') as id_cat_padre,  
                               array_length(regexp_split_to_array(coalesce(id_cat_merc,''), '[-]'),1) as livello, 
                               (case
		                        when position('-' in id_cat_merc) = 0 then id_cat_merc
		                        when position('-' in id_cat_merc) > 0 then substring(id_cat_merc from 1 for position('-' in id_cat_merc)-1)
	                            end) as ordinamento,
                                * 
                                from mc_cat_merc where id_societa = '1' and  
                                array_length(regexp_split_to_array(coalesce(id_cat_merc,''), '[-]'),1) <= 1 order by descrizione, ordinamento, id_cat_merc, livello";
                cmd.ExecuteNonQuery();

                // ricorda deve essere 3

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Categoria r = new Categoria();
                        r.id_cat_merc = reader.GetString(reader.GetOrdinal("id_cat_merc"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        r.id_cat_padre = reader.GetString(reader.GetOrdinal("id_cat_padre"));
                        r.livello = Convert.ToInt32(reader["livello"]);
                        categorie.Add(r);
                    }
                }
            }

        }

    }

    public class Categoria
    {
        public string id_cat_merc { get; set; }
        public string descrizione { get; set; }
        public string id_cat_padre { get; set; }
        public int livello { get; set; }
        public string ordinamento { get; set; }
    }

}
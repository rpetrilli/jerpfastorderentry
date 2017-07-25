﻿using Npgsql;
using System;
using System.Collections.Generic;
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

        internal void select(NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT \r\n" +
                    "      id_agente,  \r\n" +
                    "      ragione_sociale, \r\n" +
                    "       * \r\n" +
                    "from va_agenti";
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
      

    public class Agenti
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    
}
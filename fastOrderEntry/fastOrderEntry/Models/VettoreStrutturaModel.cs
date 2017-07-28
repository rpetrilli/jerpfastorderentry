using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;

namespace fastOrderEntry.Models
{
    public class VettoreStrutturaModel
    {
        public IList<Vettore> vettori { get; set; }

        internal void select(NpgsqlConnection con)
        {
            throw new NotImplementedException();
            //va_vettori
        }
    }

    public class Vettore
    {
        public string id_vettore { get; set; }
        public string ragione_sociale { get; set; }



    }
}
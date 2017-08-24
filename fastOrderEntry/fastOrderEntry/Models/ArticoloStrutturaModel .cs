using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fastOrderEntry.Models
{    
    public class ArticoloStrutturaModel
    {
        public ArticoloStrutturaModel()
        {
            this.rs = new List<Articolo>();
        }

        public virtual IList<Articolo> rs { get; set; }

        internal void select(NpgsqlConnection con, string id_cliente = "", string query = "" )
        {
            using (PetLineContext db = new PetLineContext())
            {                
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * \r\n" +
                        "from ma_articoli_soc \r\n" +
                        "where (upper(id_codice_art) like (@query) or upper(descrizione)  like (@query) ) \r\n" +
                        "order by id_codice_art asc \r\n" +
                        "limit 5";
                    
                    cmd.Parameters.AddWithValue("query",  query.ToUpper() + "%");
                    cmd.ExecuteNonQuery();

                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Articolo r = new Articolo();
                            r.id_codice_art = reader["id_codice_art"].ToString();
                            r.descrizione = reader["descrizione"].ToString();
                            r.id_iva = reader["id_iva"].ToString();
                            r.peso_lordo = Convert.ToDecimal(reader["peso_lordo"]);

                            CodiceIva codiceIva = db.codiceIva.FirstOrDefault(x => x.id_iva == r.id_iva);
                            r.aliquota = codiceIva.aliquota;

                            r.name = r.id_codice_art + " - " + r.descrizione;

                            rs.Add(r);
                        }
                    }
                }

                foreach (var r in rs)
                {
                    RecordListinoModel listinoCliente = new RecordListinoModel();
                    listinoCliente.leggiPrezziCliente(con, id_cliente);

                    RecordListinoModel listinoArticolo = new RecordListinoModel();
                    listinoCliente.leggiPrezzi(con);

                    r.prezzo_acquisto = listinoArticolo.prezzo_acquisto;
                    r.prezzo_vendita = listinoCliente.prezzo_vendita > 0 ? listinoCliente.prezzo_vendita : listinoArticolo.prezzo_vendita;
                    r.sconto_1 = listinoCliente.sconto_1 > 0 ? listinoCliente.sconto_1 : listinoArticolo.sconto_1;
                    r.sconto_2 = listinoCliente.sconto_2 > 0 ? listinoCliente.sconto_2 : listinoArticolo.sconto_2;
                    r.sconto_3 = listinoCliente.sconto_3 > 0 ? listinoCliente.sconto_3 : listinoArticolo.sconto_3;

                }
            }
                       
        }
    }

    public class Articolo
    {
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public string name { get; set; }

        public decimal prezzo_acquisto { get; set; }
        public decimal prezzo_vendita { get; set; }
        public decimal sconto_1 { get; set; }
        public decimal sconto_2 { get; set; }
        public decimal sconto_3 { get; set; }
        public string id_iva { get; set; }
        public decimal aliquota { get; set; }
        public decimal qta_ordinata { get; set; } = 1;
        public decimal qta_in_consegna { get; set; } = 0;
        public decimal peso_lordo { get; set; }

    }

    [Table("ca_iva", Schema = "public")]
    public class CodiceIva
    {
        [Key]
        public string id_iva { get; set; }
        public string descrizione { get; set; }
        public bool esenzione { get; set; }
        public decimal aliquota { get; set; }


    }
}
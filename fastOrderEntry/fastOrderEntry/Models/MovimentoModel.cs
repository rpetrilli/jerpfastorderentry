using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class MovimentoModel
    {
        public int esercizio { get; set; }
        public int nr_riga { get; set; }
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public string id_um { get; set; }
        public decimal quantita { get; set; }        
        public decimal prezzo_unitario { get; set; }
        public decimal imponibile { get; set; }
        public decimal imposta { get; set; }
        public decimal valore_riga { get; set; }
        public string str_sconto { get; set; }
        public decimal totale_riga { get; set; }
        public string id_ordine_di_vend { get; set; }
    }

    public class ListaMovimenti
    {
        public ListaMovimenti()
        {
            lista = new List<MovimentoModel>();
        }
        public int esercizio { get; set; }
        public string id_documento { get; set; }
        public string tipo { get; set; }
        public virtual IList<MovimentoModel> lista { get; set; }

        public void select(NpgsqlConnection con)
        {
            lista = new List<MovimentoModel>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.Connection.Open();

                switch (tipo)
                {
                    case "F":
                        cmd.CommandText = @"select * from vo_fatture_righe where esercizio= @esercizio and id_fattura= @id_documento";
                        break;
                    case "C":
                        cmd.CommandText = @"select * from vo_consegne_righe where esercizio= @esercizio and id_consegna= @id_documento";
                        break;
                }

                cmd.Parameters.AddWithValue("esercizio", esercizio);
                cmd.Parameters.AddWithValue("id_documento", id_documento);                
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MovimentoModel mov = new MovimentoModel()
                        {
                            esercizio = setInt(reader["esercizio"].ToString()),
                            descrizione = reader["descrizione"].ToString(),
                            id_codice_art = reader["id_codice_art"].ToString().TrimStart('0'),
                            //id_ordine_di_vend = reader["id_ordine_di_vend"].ToString().TrimStart('0'),
                            //id_um = reader["id_um"].ToString(),
                            imponibile = setDecimal(reader["imponibile"].ToString()),
                            imposta = setDecimal(reader["imposta"].ToString()),
                            nr_riga = setInt(reader["nr_riga"].ToString()),
                            prezzo_unitario = setDecimal(reader["prezzo_unitario"].ToString()),
                            quantita = setDecimal(reader["quantita"].ToString()),
                            str_sconto = reader["str_sconto"].ToString(),
                            totale_riga = setDecimal(reader["totale_riga"].ToString()),
                            //valore_riga = setDecimal(reader["valore_riga"].ToString())
                        };
                        lista.Add(mov);
                    }
                }

                cmd.Connection.Close();
            }
        }

        private string GetDate(string data)
        {
            try
            {
                return Convert.ToDateTime(data).ToShortDateString();
            }
            catch
            {
                return data;
            }
        }

        private decimal setDecimal(string valore)
        {
            return string.IsNullOrEmpty(valore) ? 0 : Convert.ToDecimal(valore);
        }

        private int setInt(string valore)
        {
            return string.IsNullOrEmpty(valore) ? 0 : Convert.ToInt32(valore);
        }
    }

}
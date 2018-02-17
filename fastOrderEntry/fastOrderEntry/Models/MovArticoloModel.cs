using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class MovArticoloModel
    {
        public int esercizio { get; set; }
        public string id_documento { get; set; }
        public string id_fornitore { get; set; }
        public string rag_soc_fornitore { get; set; }
        public string rag_soc_cliente { get; set; }
        public string id_cliente { get; set; }
        public string tipo_documento { get; set; }
        public string data_documento { get; set; }
        public int protocollo_iva { get; set; }
        public decimal totale_doc { get; set; }
        public decimal imponibile_doc { get; set; }
        public decimal imposta_doc { get; set; }
        public string nr_documento_merce { get; set; }
        public int nr_riga { get; set; }
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public decimal quantita { get; set; }
        public string id_um { get; set; }
        public decimal prezzo_unitario { get; set; }
        public string str_sconto { get; set; }
        public decimal imponibile { get; set; }
        public decimal imposta { get; set; }
        public decimal totale_riga { get; set; }
        public string id_magazzino { get; set; }
        public string segno { get; set; }        
    }

    public class ListaMovArticoloModel
    {
        public ListaMovArticoloModel()
        {
            lista = new List<MovArticoloModel>();
        }
        public string id_codice_art { get; set; }
        public virtual IList<MovArticoloModel> lista { get; set; }

        public void select(NpgsqlConnection con, DateTime da_data, DateTime a_data)
        {
            lista = new List<MovArticoloModel>();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.Connection.Open();
                cmd.CommandText = @"select mov_articolo.*,
                aa_fornitori.ragione_sociale as rag_soc_fornitore,
                va_clienti.ragione_sociale as rag_soc_cliente
                from mov_articolo
                left join aa_fornitori on
                aa_fornitori.id_fornitore = mov_articolo.id_fornitore
                left join va_clienti on
                va_clienti.id_cliente = mov_articolo.id_cliente
                where id_codice_art = @id_codice_art and (data_documento >= @da_data and data_documento <= @a_data)";

                

                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.Parameters.AddWithValue("da_data", da_data);
                cmd.Parameters.AddWithValue("a_data", a_data);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MovArticoloModel mov = new MovArticoloModel();

                        mov.esercizio = setInt(reader["esercizio"].ToString());
                        mov.id_documento = reader["id_documento"].ToString().TrimStart('0');
                        mov.id_fornitore = reader["id_fornitore"].ToString();
                        mov.id_cliente = reader["id_cliente"].ToString();
                        mov.tipo_documento = reader["tipo_documento"].ToString();
                        mov.data_documento = GetDate(reader["data_documento"].ToString());
                        mov.protocollo_iva = setInt(reader["protocollo_iva"].ToString().TrimStart('0'));
                        mov.totale_doc = setDecimal(reader["totale_doc"].ToString());
                        mov.imponibile_doc = setDecimal(reader["imponibile_doc"].ToString());
                        mov.imposta_doc = setDecimal(reader["imposta_doc"].ToString());
                        mov.nr_documento_merce = reader["nr_documento_merce"].ToString();
                        mov.nr_riga = setInt(reader["nr_riga"].ToString());
                        mov.id_codice_art = reader["id_codice_art"].ToString();
                        mov.descrizione = reader["descrizione"].ToString();
                        mov.quantita = setDecimal(reader["quantita"].ToString());
                        mov.id_um = reader["id_um"].ToString();
                        mov.prezzo_unitario = setDecimal(reader["prezzo_unitario"].ToString());
                        mov.str_sconto = reader["str_sconto"].ToString();
                        mov.imponibile = setDecimal(reader["imponibile"].ToString());
                        mov.imposta = setDecimal(reader["imposta"].ToString());
                        mov.totale_riga = setDecimal(reader["totale_riga"].ToString());
                        mov.id_magazzino = reader["id_magazzino"].ToString();
                        mov.segno = reader["segno"].ToString();
                        mov.rag_soc_fornitore = reader["rag_soc_fornitore"].ToString();
                        mov.rag_soc_cliente = reader["rag_soc_cliente"].ToString();

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
                return Convert.ToDateTime(data).ToString("yyyy-MM-dd");
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
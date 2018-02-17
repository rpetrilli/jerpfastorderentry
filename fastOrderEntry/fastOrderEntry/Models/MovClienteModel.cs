using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;

namespace fastOrderEntry.Models
{
    public class RecordClienteModel
    {
        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        public string da_data { get; set; }
        public string a_data { get; set; }
    }

    public class MovClienteModel
    {
        public string tipo { get; set; }
        public string id_societa { get; set; }
        public string id_divisione { get; set; }
        public string tipo_documento { get; set; }
        public int esercizio { get; set; }
        public string id_documento { get; set; }
        public string id { get; set; }
        public int protocollo_iva { get; set; }
        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        public decimal totale_doc { get; set; }
        public decimal imponibile { get; set; }
        public decimal imposta { get; set; }
        public string nr_documento { get; set; }        
        public int esercizio_fatt { get; set; }
        public string id_fattura { get; set; }
        public string protocollo_iva_fatt { get; set; }
        public string data_registrazione { get; set; }
        public string data_documento { get; set; }
        public bool show { get; set; }
    }

    public class ListaMovClienteModel
    {
        public ListaMovClienteModel()
        {
            lista = new List<MovClienteModel>();
        }

        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        public virtual IList<MovClienteModel> lista { get; set; }

        public void select(NpgsqlConnection con, DateTime da_data, DateTime a_data)
        {
            lista = new List<MovClienteModel>();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.Connection.Open();
                cmd.CommandText = @"select * from mov_cliente where id_cliente = @id_cliente and (data_documento >= @da_data and data_documento <= @a_data)";

                cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                cmd.Parameters.AddWithValue("da_data", da_data);
                cmd.Parameters.AddWithValue("a_data", a_data);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MovClienteModel mov = new MovClienteModel()
                        {
                            tipo = reader["tipo"].ToString(),
                            
                            data_documento = GetDate(reader["data_documento"].ToString()),
                            data_registrazione = GetDate(reader["data_documento"].ToString()),
                            esercizio = setInt(reader["esercizio"].ToString()),
                            id_cliente = reader["id_cliente"].ToString(),
                            id_documento = reader["id_documento"].ToString().TrimStart('0'),
                            id = reader["id_documento"].ToString(),
                            imponibile = setDecimal(reader["imponibile"].ToString()),
                            imposta = setDecimal(reader["imposta"].ToString()),
                            nr_documento = reader["id_documento"].ToString().TrimStart('0'),
                            protocollo_iva = setInt(reader["protocollo_iva"].ToString()),
                            ragione_sociale = reader["ragione_sociale"].ToString(),
                            tipo_documento = reader["tipo_documento"].ToString(),
                            totale_doc = setDecimal(reader["totale_doc"].ToString()),
                            show = true
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
using Npgsql;

namespace fastOrderEntry.Models
{
    public class RecordListino
    {
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }
        public decimal prezzo_acquisto { get; set; }
        public decimal prezzo_vendita { get; set; }
        public decimal sconto_1 { get; set; }
        public decimal sconto_2 { get; set; }
        public decimal sconto_3 { get; set; }

        internal void leggiPrezzi(NpgsqlConnection conn)
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_listini_articolo where id_cond_prezzo = 'VA01' and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        prezzo_vendita = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
            }
            
            conn.Close();
        }
    }
}
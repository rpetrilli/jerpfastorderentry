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
  

            using (var cmd = new NpgsqlCommand())
            {
                prezzo_vendita = 0;
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_listini_articolo where id_cond_prezzo = 'VA01' and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prezzo_vendita = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }

            using (var cmd = new NpgsqlCommand())
            {
                prezzo_acquisto = 0;
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_listini_articolo where id_cond_prezzo = 'AC01' and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prezzo_acquisto = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }


            using (var cmd = new NpgsqlCommand())
            {
                sconto_1 = 0;
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_sconti_articolo where id_cond_prezzo = 'SC01' and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sconto_1 = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }

            using (var cmd = new NpgsqlCommand())
            {
                sconto_2 = 0;
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_sconti_articolo where id_cond_prezzo = 'SC02' and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sconto_2 = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }

            using (var cmd = new NpgsqlCommand())
            {
                sconto_3 = 0;
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_sconti_articolo where id_cond_prezzo = 'SC03' and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sconto_3 = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }


        }
    }
}
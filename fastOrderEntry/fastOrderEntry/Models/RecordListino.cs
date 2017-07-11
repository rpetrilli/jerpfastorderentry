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

        internal void scriviPrezzi(NpgsqlConnection conn)
        {

            int cnt = 0;
            using (var cmd = new NpgsqlCommand())
            {
            
                cmd.Connection = conn;
                cmd.CommandText = "update da_listini_articolo set val_condizione = @val_condizione " +
                    "where id_cond_prezzo = 'VA01' " +
                    "  and id_codice_art = @id_codice_art and id_divisa = 'EUR' ";
                cmd.Parameters.AddWithValue("val_condizione", prezzo_vendita);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cnt = cmd.ExecuteNonQuery();
            }
            if (cnt == 0)
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO da_listini_articolo( \r\n"+
                        "            id_societa, id_cond_prezzo, id_divisa, id_codice_art, id_um, \r\n"+ 
                        "            val_condizione) \r\n"+
                        "    VALUES('1', 'VA01', 'EUR', @id_codice_art, 'PZ', \r\n"+
                        "            @val_condizione)";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("val_condizione", prezzo_vendita);
                    cnt = cmd.ExecuteNonQuery();
                }
            }  //Fine vendita


            cnt = 0;
            using (var cmd = new NpgsqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "update da_listini_articolo set val_condizione = @val_condizione " +
                    "where id_cond_prezzo = 'AC01' " +
                    "  and id_codice_art = @id_codice_art and id_divisa = 'EUR' ";
                cmd.Parameters.AddWithValue("val_condizione", prezzo_acquisto);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cnt = cmd.ExecuteNonQuery();
            }
            if (cnt == 0)
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO da_listini_articolo( \r\n" +
                        "            id_societa, id_cond_prezzo, id_divisa, id_codice_art, id_um, \r\n" +
                        "            val_condizione) \r\n" +
                        "    VALUES('1', 'AC01', 'EUR', @id_codice_art, 'PZ', \r\n" +
                        "            @val_condizione)";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("val_condizione", prezzo_acquisto);
                    cnt = cmd.ExecuteNonQuery();
                }
            }

            cnt = 0;
            using (var cmd = new NpgsqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "update da_sconti_articolo set val_condizione = @val_condizione " +
                    "where id_cond_prezzo = 'SC01' " +
                    "  and id_codice_art = @id_codice_art ";
                cmd.Parameters.AddWithValue("val_condizione", sconto_1);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cnt = cmd.ExecuteNonQuery();
            }
            if (cnt == 0)
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO da_sconti_articolo( \r\n" +
                        "            id_societa, id_cond_prezzo, id_codice_art,  \r\n" +
                        "            val_condizione) \r\n" +
                        "    VALUES('1', 'SC01',  @id_codice_art, \r\n" +
                        "            @val_condizione)";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("val_condizione", sconto_1);
                    cnt = cmd.ExecuteNonQuery();
                }
            }  //Fine sconto 1

            cnt = 0;
            using (var cmd = new NpgsqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "update da_sconti_articolo set val_condizione = @val_condizione " +
                    "where id_cond_prezzo = 'SC02' " +
                    "  and id_codice_art = @id_codice_art ";
                cmd.Parameters.AddWithValue("val_condizione", sconto_2);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cnt = cmd.ExecuteNonQuery();
            }
            if (cnt == 0)
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO da_sconti_articolo( \r\n" +
                        "            id_societa, id_cond_prezzo, id_codice_art,  \r\n" +
                        "            val_condizione) \r\n" +
                        "    VALUES('1', 'SC02',  @id_codice_art, \r\n" +
                        "            @val_condizione)";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("val_condizione", sconto_2);
                    cnt = cmd.ExecuteNonQuery();
                }
            }  //Fine sconto 2

            cnt = 0;
            using (var cmd = new NpgsqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "update da_sconti_articolo set val_condizione = @val_condizione " +
                    "where id_cond_prezzo = 'SC03' " +
                    "  and id_codice_art = @id_codice_art ";
                cmd.Parameters.AddWithValue("val_condizione", sconto_3);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cnt = cmd.ExecuteNonQuery();
            }
            if (cnt == 0)
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO da_sconti_articolo( \r\n" +
                        "            id_societa, id_cond_prezzo, id_codice_art,  \r\n" +
                        "            val_condizione) \r\n" +
                        "    VALUES('1', 'SC03',  @id_codice_art, \r\n" +
                        "            @val_condizione)";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("val_condizione", sconto_3);
                    cnt = cmd.ExecuteNonQuery();
                }
            }  //Fine sconto 3


        }
    }
}
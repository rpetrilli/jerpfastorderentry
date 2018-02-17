using System;
using Npgsql;

namespace fastOrderEntry.Models
{
    public class RecordListinoModel
    {
        public string id_codice_art { get; set; }
        public string id_cliente { get; set; }
        public string descrizione { get; set; }
        public decimal prezzo_acquisto { get; set; }
        public decimal prezzo_vendita { get; set; }
        public decimal sconto_1 { get; set; }
        public decimal sconto_2 { get; set; }
        public decimal sconto_3 { get; set; }
        public decimal sconto_a_1 { get; set; }
        public decimal sconto_a_2 { get; set; }
        public decimal sconto_a_3 { get; set; }
        public decimal sconto_agente { get; set; }
        public decimal giacenza { get; set; }
        public string id_iva { get; set; }
        public string cod_fornitore { get; set; }

        internal void leggiPrezzi(NpgsqlConnection conn)
        {
            prezzo_vendita = leggiListinoArticolo(conn, "VA01");
            prezzo_acquisto = leggiPrezzoAcquisto(conn, "AC01");

            sconto_1 = leggiSconto(conn, "SC01");
            sconto_2 = leggiSconto(conn, "SC02");
            sconto_3 = leggiSconto(conn, "SC03");

            sconto_a_1 = leggiSconto(conn, "SA01");
            sconto_a_2 = leggiSconto(conn, "SA02");
            sconto_a_3 = leggiSconto(conn, "SA03");
        }


        public void leggiPrezziCliente(NpgsqlConnection conn, string id_cliente)
        {
            this.id_cliente = id_cliente;
            prezzo_vendita = leggiListinoCliente(conn, "VA01", id_cliente);

            sconto_1 = leggiScontoCliente(conn, "SC01", id_cliente);
            sconto_2 = leggiScontoCliente(conn, "SC02", id_cliente);
            sconto_3 = leggiScontoCliente(conn, "SC03", id_cliente);
        }

        private decimal leggiSconto(NpgsqlConnection conn, string id_cond_prezzo)
        {
            decimal sconto = 0;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_sconti_articolo where id_cond_prezzo = @id_cond_prezzo and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sconto = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }
            return sconto;
        }

        private decimal leggiScontoCliente(NpgsqlConnection conn, string id_cond_prezzo, string id_cliente)
        {
            decimal sconto = 0;
            if (string.IsNullOrWhiteSpace(id_cliente))
            {
                return 0;
            }
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_sconti_cliente_articolo " +
                    "where id_cond_prezzo = @id_cond_prezzo " +
                    "  and id_codice_art = @id_codice_art " +
                    "  and id_cliente = @id_cliente " +
                    "limit 1";
                cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sconto = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }
            return sconto;
        }

        private decimal leggiPrezzoAcquisto(NpgsqlConnection conn, string id_cond_prezzo)
        {
            decimal prz = 0;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"select 
                        case when ao_accettazione_righe.quantita<>0 then 
                        ao_accettazione_righe.imponibile/ao_accettazione_righe.quantita else 
                        ao_accettazione_righe.prezzo_unitario end as ult_prezzo_acq
                        from ao_accettazione_righe
                        where ao_accettazione_righe.id_codice_art=(@query)
                        order by ao_accettazione_righe.esercizio desc,
                        ao_accettazione_righe.id_accettazione desc limit 1";
               
                cmd.Parameters.AddWithValue("query", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prz = reader.GetDecimal(reader.GetOrdinal("ult_prezzo_acq"));
                    }
                }
            }
            return prz;
        }

        private decimal leggiListinoArticolo(NpgsqlConnection conn, string id_cond_prezzo)
        {
            decimal prz = 0;
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_listini_articolo where id_cond_prezzo = @id_cond_prezzo and id_codice_art = @id_codice_art limit 1";
                cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prz = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }
            return prz;
        }


        private decimal leggiListinoCliente(NpgsqlConnection conn, string id_cond_prezzo, string id_cliente)
        {
            decimal prz = 0;

            if (string.IsNullOrEmpty(id_cliente))
            {
                return 0;
            }

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * from da_listini_cliente \r\n" +
                    "where id_cond_prezzo = @id_cond_prezzo \r\n" +
                    "  and id_codice_art = @id_codice_art \r\n" +
                    "  and id_cliente = @id_cliente \r\n" +
                    "limit 1";
                cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prz = reader.GetDecimal(reader.GetOrdinal("val_condizione"));
                    }
                }
            }
            return prz;
        }

        internal void scriviPrezzi(NpgsqlConnection conn)
        {

            updatePrezzo(conn, prezzo_vendita, "VA01");
            updatePrezzo(conn, prezzo_acquisto, "AC01");

            updateSconto(conn, sconto_1, "SC01");
            updateSconto(conn, sconto_2, "SC02");
            updateSconto(conn, sconto_3, "SC03");
            
        }

        internal void scriviPrezziAcq(NpgsqlConnection conn)
        {
            updatePrezzo(conn, prezzo_acquisto, "AC01");

            updateSconto(conn, sconto_a_1, "SA01");
            updateSconto(conn, sconto_a_2, "SA02");
            updateSconto(conn, sconto_a_3, "SA03");
        }


        internal void scriviPrezziCliente(NpgsqlConnection con)
        {
            updatePrezzoCliente(con, prezzo_vendita, "VA01");

            updateScontoCliente(con, sconto_1, "SC01");
            updateScontoCliente(con, sconto_2, "SC02");
            updateScontoCliente(con, sconto_3, "SC03");
        }


        public void updatePrezzo(NpgsqlConnection conn, decimal prezzo, string id_cond_prezzo)
        {
            int cnt = 0;
            if (prezzo > 0)
            {

                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "update da_listini_articolo set val_condizione = @val_condizione " +
                        "where id_cond_prezzo = @id_cond_prezzo " +
                        "  and id_codice_art = @id_codice_art and id_divisa = 'EUR' ";
                    cmd.Parameters.AddWithValue("val_condizione", prezzo);
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
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
                            "    VALUES('1', @id_cond_prezzo, 'EUR', @id_codice_art, 'PZ', \r\n" +
                            "            @val_condizione)";
                        cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("val_condizione", prezzo);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM da_listini_articolo \r\n" +
                        "   where id_societa = '1' and  id_cond_prezzo = @id_cond_prezzo \r\n" +
                        "     and id_codice_art =  @id_codice_art ";
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cnt = cmd.ExecuteNonQuery();
                }
            }//Fine vendita

        }

        public void updateSconto(NpgsqlConnection conn, decimal sconto, string id_cond_prezzo)
        {
            int cnt = 0;
            if (sconto > 0)
            {
                cnt = 0;
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "update da_sconti_articolo set val_condizione = @val_condizione " +
                        "where id_cond_prezzo = @id_cond_prezzo " +
                        "  and id_codice_art = @id_codice_art ";
                    cmd.Parameters.AddWithValue("val_condizione", sconto);
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
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
                            "    VALUES('1', @id_cond_prezzo,  @id_codice_art, \r\n" +
                            "            @val_condizione)";
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                        cmd.Parameters.AddWithValue("val_condizione", sconto);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM da_sconti_articolo \r\n" +
                        "   where id_societa = '1' and  id_cond_prezzo = @id_cond_prezzo \r\n" +
                        "     and id_codice_art =  @id_codice_art ";
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cnt = cmd.ExecuteNonQuery();
                }
            }

        }


        public void updatePrezzoCliente(NpgsqlConnection conn, decimal prezzo, string id_cond_prezzo)
        {
            int cnt = 0;
            if (prezzo > 0)
            {

                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "update da_listini_cliente set val_condizione = @val_condizione " +
                        "where id_cond_prezzo = @id_cond_prezzo " +
                        "  and id_codice_art = @id_codice_art " +
                        "  and id_cliente = @id_cliente " +
                        "  and id_divisa = 'EUR' ";
                    cmd.Parameters.AddWithValue("val_condizione", prezzo);
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                    cnt = cmd.ExecuteNonQuery();
                }
                if (cnt == 0)
                {
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO da_listini_cliente( \r\n" +
                            "            id_societa, id_cond_prezzo, id_divisa, id_codice_art, id_um, \r\n" +
                            "            val_condizione, id_cliente) \r\n" +
                            "    VALUES('1', @id_cond_prezzo, 'EUR', @id_codice_art, 'PZ', \r\n" +
                            "            @val_condizione, @id_cliente)";
                        cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("val_condizione", prezzo);
                        cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM da_listini_cliente \r\n" +
                        "   where id_societa = '1' and  id_cond_prezzo = @id_cond_prezzo \r\n" +
                        "     and id_codice_art =  @id_codice_art " +
                        "     and id_cliente =  @id_cliente ";
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                    cnt = cmd.ExecuteNonQuery();
                }
            }//Fine vendita

        }

        public void updateScontoCliente(NpgsqlConnection conn, decimal sconto, string id_cond_prezzo)
        {
            int cnt = 0;
            if (sconto > 0)
            {
                cnt = 0;
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "update da_sconti_cliente_articolo set val_condizione = @val_condizione " +
                        "where id_cond_prezzo = @id_cond_prezzo " +
                        "  and id_codice_art = @id_codice_art " +
                        "  and id_cliente = @id_cliente ";
                    cmd.Parameters.AddWithValue("val_condizione", sconto);
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                    cnt = cmd.ExecuteNonQuery();
                }
                if (cnt == 0)
                {
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO da_sconti_cliente_articolo( \r\n" +
                            "            id_societa, id_cond_prezzo, id_codice_art,  \r\n" +
                            "            val_condizione, id_cliente) \r\n" +
                            "    VALUES('1', @id_cond_prezzo,  @id_codice_art, \r\n" +
                            "            @val_condizione, @id_cliente)";
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                        cmd.Parameters.AddWithValue("val_condizione", sconto);
                        cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM da_sconti_cliente_articolo \r\n" +
                        "   where id_societa = '1' and  id_cond_prezzo = @id_cond_prezzo \r\n" +
                        "     and id_codice_art =  @id_codice_art " +
                        "     and id_cliente = @id_cliente ";
                    cmd.Parameters.AddWithValue("id_cond_prezzo", id_cond_prezzo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("id_cliente", id_cliente);
                    cnt = cmd.ExecuteNonQuery();
                }
            }

        }
    }



}





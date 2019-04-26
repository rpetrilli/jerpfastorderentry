using System;
using System.Collections.Generic;
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
        public decimal prezzo_listino { get; set; }
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
        public string codice_ean { get; set; }
        public string id_fornitore { get; set; }
        public bool obosleto { get; set; }
        public decimal peso_netto { get; set; }
        public string um_peso { get; set; }

        internal void leggiPrezzi(NpgsqlConnection conn, bool lisAcq = false)
        {
            prezzo_vendita = leggiListinoArticolo(conn, "VA01");
            prezzo_acquisto = leggiPrezzoAcquisto(conn, "AC01", false);
            prezzo_listino = leggiPrezzoAcquisto(conn, "AC01", true);
            //prezzo_acquisto = leggiListinoArticolo(conn, "VA01");

            sconto_1 = leggiSconto(conn, "SC01");
            sconto_2 = leggiSconto(conn, "SC02");
            sconto_3 = leggiSconto(conn, "SC03");

            sconto_a_1 = leggiSconto(conn, "SA01");
            sconto_a_2 = leggiSconto(conn, "SA02");
            sconto_a_3 = leggiSconto(conn, "SA03");
        }


        public void leggiPrezziCliente(NpgsqlConnection conn, string id_cliente, bool lisAcq = false)
        {
            this.id_cliente = id_cliente;
            prezzo_vendita = leggiListinoCliente(conn, "VA01", id_cliente);
            prezzo_acquisto = leggiPrezzoAcquisto(conn, "AC01", lisAcq);
            prezzo_listino = leggiListinoArticolo(conn, "VA01");

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

        private decimal leggiPrezzoAcquisto(NpgsqlConnection conn, string id_cond_prezzo, bool lisAcq)
        {
            decimal prz = 0;
            //prz = GetFromListino(conn, id_cond_prezzo);
            //prz = GetFromAccettazione(conn, id_cond_prezzo);

            if (lisAcq)
                prz = GetFromListino(conn, id_cond_prezzo);
            else
                prz = GetFromAccettazione(conn, id_cond_prezzo);          

            return prz;
        }

        private decimal GetFromAccettazione(NpgsqlConnection conn, string id_cond_prezzo)
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
                        left join ao_accettazione on
                        ao_accettazione.id_divisione = ao_accettazione_righe.id_divisione and
                        ao_accettazione.esercizio = ao_accettazione_righe.esercizio and
                        ao_accettazione.id_accettazione = ao_accettazione_righe.id_accettazione
                        where ao_accettazione.id_tipo_accettazione <> 'RC' and
                        ao_accettazione_righe.id_codice_art=(@query)
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

        private decimal GetFromListino(NpgsqlConnection conn, string id_cond_prezzo)
        {
            decimal prz = 0;
            using (var cmd = new NpgsqlCommand())
            {


                cmd.Connection = conn;
                cmd.CommandText = "select * from da_listini_articolo " +
                    "where id_cond_prezzo = @id_cond_prezzo " +
                    "  and id_codice_art = @id_codice_art and id_divisa = 'EUR' ";
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

        internal void updatePesoNetto(NpgsqlConnection con, decimal? peso_netto_massivo, string query, string cod_cat_merc)
        {
            List<RecordListinoModel> recordlistino = new List<RecordListinoModel>();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "SELECT * from ma_articoli_soc \r\n" +
                    "where id_societa = '1' \r\n";
                if (!string.IsNullOrEmpty(query))
                {
                    cmd.CommandText += "  and (upper(id_codice_art) LIKE( @query) or upper(descrizione) like( @query ) ) \r\n";
                }
                if (!string.IsNullOrEmpty(cod_cat_merc))
                {
                    cmd.CommandText += " and (id_categoria_merc like ('" + cod_cat_merc + "-%') or id_categoria_merc ='" + cod_cat_merc + "')";
                }

                if (!string.IsNullOrEmpty(query))
                {
                    cmd.Parameters.AddWithValue("query", query.ToUpper() + "%");
                }
                cmd.ExecuteNonQuery();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecordListinoModel r = new RecordListinoModel();
                        r.id_codice_art = reader.GetString(reader.GetOrdinal("id_codice_art"));
                        r.descrizione = reader.GetString(reader.GetOrdinal("descrizione"));
                        recordlistino.Add(r);
                    }
                }

                foreach (RecordListinoModel r in recordlistino)
                {
                    r.updateDescrizioneMassivo(con, Convert.ToDecimal(peso_netto_massivo));
                }
            }
        }

        internal void AggiornaArticolo(NpgsqlConnection con)
        {
            updateDescrizione(con);
            updateEan(con);
            updateCodFornitore(con);
           
        }

        private void updateObsoleto(NpgsqlConnection con)
        {
            throw new NotImplementedException();
        }

        private void updateCodFornitore(NpgsqlConnection con)
        {
            int cnt = 0;
            if (!string.IsNullOrEmpty(cod_fornitore))
            {

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "update aa_source_list set codice_fornitore = @cod_fornitore,  preferenziale = true \r\n" +
                        " where id_divisione ='1' and id_cod_articolo = @id_codice_art and id_societa = '1' and id_fornitore = @id_fornitore";
                    cmd.Parameters.AddWithValue("id_fornitore", id_fornitore);
                    cmd.Parameters.AddWithValue("cod_fornitore", cod_fornitore);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cnt = cmd.ExecuteNonQuery();
                }
                if (cnt == 0)
                {
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM aa_source_list \r\n" +
                            " where id_divisione ='1' and id_cod_articolo = @id_codice_art and id_societa = '1'";
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cnt = cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO aa_source_list( \r\n" +
                            "            id_divisione, id_cod_articolo, id_fornitore, id_societa, preferenziale, \r\n" +
                            "            codice_fornitore) \r\n" +
                            "    VALUES('1', @id_codice_art, @id_fornitore, '1', true, @codice_fornitore)";
                        cmd.Parameters.AddWithValue("id_fornitore", id_fornitore);
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("codice_fornitore", cod_fornitore);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void updateEan(NpgsqlConnection con)
        {
            int cnt = 0;
            if (!string.IsNullOrEmpty(codice_ean))
            {

                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = con;
                    cmd.CommandText = "update ma_articoli_ean set codice_ean = @codice_ean \r\n" +
                        " where id_societa = '1'  and id_codice_art = @id_codice_art";
                    cmd.Parameters.AddWithValue("codice_ean", codice_ean);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cnt = cmd.ExecuteNonQuery();
                }
                if (cnt == 0)
                {
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO ma_articoli_ean( \r\n" +
                            "            id_societa, id_codice_art, codice_ean, id_um) \r\n" +
                            "    VALUES('1', @id_codice_art, @codice_ean, 'PZ')";
                        cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                        cmd.Parameters.AddWithValue("codice_ean", codice_ean);
                        cnt = cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var cmd = new NpgsqlCommand())
                {

                    cmd.Connection = con;
                    cmd.CommandText = "DELETE FROM ma_articoli_ean \r\n" +
                        "   where id_societa = '1' and id_codice_art =  @id_codice_art ";
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cnt = cmd.ExecuteNonQuery();
                }
            }//Fine vendita
        }

        private void updateDescrizione(NpgsqlConnection con)
        {
            if (!string.IsNullOrEmpty(id_codice_art))
            {

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "update ma_articoli_soc set descrizione = @descrizione, obsoleto = @obsoleto, peso_netto = @peso_netto " +
                        "where id_codice_art = @id_codice_art ";
                    cmd.Parameters.AddWithValue("descrizione", descrizione);
                    cmd.Parameters.AddWithValue("obsoleto", obosleto);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.Parameters.AddWithValue("peso_netto", peso_netto);
                    cmd.ExecuteNonQuery();
                }

            }
        }


        private void updateDescrizioneMassivo(NpgsqlConnection con, decimal peso_netto_massivo)
        {
            if (!string.IsNullOrEmpty(id_codice_art))
            {

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "update ma_articoli_soc set peso_netto = @peso_netto " +
                        "where id_codice_art = @id_codice_art ";
                    cmd.Parameters.AddWithValue("peso_netto", peso_netto_massivo);
                    cmd.Parameters.AddWithValue("id_codice_art", id_codice_art);
                    cmd.ExecuteNonQuery();
                }

            }
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
            //updatePrezzo(conn, prezzo_acquisto, "AC01");
            //commentato per errore su aggiornamento

            updateSconto(conn, sconto_1, "SC01");
            updateSconto(conn, sconto_2, "SC02");
            updateSconto(conn, sconto_3, "SC03");
            
        }

        internal void scriviPrezziAcq(NpgsqlConnection conn)
        {
            //updatePrezzo(conn, prezzo_acquisto, "AC01");
            updatePrezzo(conn, prezzo_listino, "AC01");

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





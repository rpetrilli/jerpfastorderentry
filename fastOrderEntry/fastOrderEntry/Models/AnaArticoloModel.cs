using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class AnaArticoloModel
    {
        public String id_codice_art_rif { get; set; }
        public String id_codice_art { get; set; }
        public String descrizione { get; set; }
        public String id_um { get; set; }
        public decimal peso_lordo { get; set; }
        public decimal prezzo_acquisto { get; set; }
        public decimal prezzo_vendita { get; set; }
        public decimal sconto_1 { get; set; }
        public decimal sconto_2 { get; set; }
        public decimal sconto_3 { get; set; }
        public List<CondizioneCliente> condizioni_cliente { get; set; }

        public void select()
        {
            using (PetLineContext db = new PetLineContext())
            {
                var query = from art in db.ma_articoli_soc
                            where art.id_codice_art.Equals(this.id_codice_art_rif)
                            select art;
                var articoli_soc = query.FirstOrDefault<ma_articoli_soc>();

                descrizione = articoli_soc.descrizione;
                id_um = articoli_soc.id_um;
                peso_lordo = articoli_soc.peso_lordo;

                prezzo_acquisto = getValoreCond(db, "AC01", this.id_codice_art_rif);
                prezzo_vendita = getValoreCond(db, "VA01", this.id_codice_art_rif);

                sconto_1 = getValoreSconto(db, "SC01", this.id_codice_art_rif);
                sconto_2 = getValoreSconto(db, "SC02", this.id_codice_art_rif);
                sconto_3 = getValoreSconto(db, "SC03", this.id_codice_art_rif);

                string sql = @"select va_clienti.* from (select distinct * from (
                    select id_cliente from da_sconti_cliente_articolo where id_codice_art = @id_codice_art_rif
                    union
                    select id_cliente from da_listini_cliente where id_codice_art = @id_codice_art_rif) as tab
                    ) as elenco_clienti
                    inner join va_clienti
                    on elenco_clienti.id_cliente = va_clienti.id_cliente";

                condizioni_cliente = db.CondizioneCliente.SqlQuery(sql, new NpgsqlParameter("id_codice_art_rif", this.id_codice_art_rif) ).ToList();

                foreach (var item in condizioni_cliente)
                {
                    item.prezzo_acquisto = getValoreCondCliente(db, "AC01", this.id_codice_art_rif, item.id_cliente);
                    item.prezzo_vendita = getValoreCondCliente(db, "VA01", this.id_codice_art_rif, item.id_cliente);

                    item.sconto_1 = getValoreScontoCliente(db, "SC01", this.id_codice_art_rif, item.id_cliente);
                    item.sconto_2 = getValoreScontoCliente(db, "SC02", this.id_codice_art_rif, item.id_cliente);
                    item.sconto_3 = getValoreScontoCliente(db, "SC03", this.id_codice_art_rif, item.id_cliente);
                }


            }
        }

        private decimal getValoreCond(PetLineContext db, String id_cond_prezzo, String id_codice_art)
        {
            try
            {
                da_listini_articolo listino = db.da_listini_articolo
                    .FirstOrDefault(x => x.id_codice_art == id_codice_art && x.id_cond_prezzo == id_cond_prezzo);
                return listino.val_condizione;
            }
            catch {
                return 0;
            }
        }

        private decimal getValoreSconto(PetLineContext db, String id_cond_prezzo, String id_codice_art)
        {
            try
            {
                da_sconti_articolo sconto = db.da_sconti_articolo
                    .FirstOrDefault(x => x.id_codice_art == id_codice_art && x.id_cond_prezzo == id_cond_prezzo);
                return sconto.val_condizione;
            }
            catch
            {
                return 0;
            }
        }

        private decimal getValoreCondCliente(PetLineContext db, String id_cond_prezzo, String id_codice_art, String id_cliente)
        {
            try
            {
                da_listini_cliente listino = db.da_listini_cliente
                    .FirstOrDefault(x => x.id_codice_art == id_codice_art && x.id_cond_prezzo == id_cond_prezzo && x.id_cliente == id_cliente);
                return listino.val_condizione;
            }
            catch
            {
                return 0;
            }
        }

        private decimal getValoreScontoCliente(PetLineContext db, String id_cond_prezzo, String id_codice_art, String id_cliente)
        {
            try
            {
                da_sconti_cliente_articolo sconto = db.da_sconti_cliente_articolo
                    .FirstOrDefault(x => x.id_codice_art == id_codice_art && x.id_cond_prezzo == id_cond_prezzo && x.id_cliente == id_cliente);
                return sconto.val_condizione;
            }
            catch
            {
                return 0;
            }
        }

    }

    [Table("va_clienti", Schema = "public")]
    public class CondizioneCliente
    {
        [Key]
        public string id_cliente { get; set; }
        public string ragione_sociale { get; set; }
        [NotMapped]
        public decimal prezzo_acquisto { get; set; }
        [NotMapped]
        public decimal prezzo_vendita { get; set; }
        [NotMapped]
        public decimal sconto_1 { get; set; }
        [NotMapped]
        public decimal sconto_2 { get; set; }
        [NotMapped]
        public decimal sconto_3 { get; set; }
    }


    [Table("ma_articoli_soc", Schema = "public")]
    public class ma_articoli_soc
    {
        [Key]
        public String id_codice_art { get; set; }
        public String descrizione { get; set; }
        public String id_um { get; set; }
        public decimal peso_lordo { get; set; }


    }

    [Table("da_listini_articolo", Schema = "public")]
    public class da_listini_articolo
    {
        [Key]
        public long id { get; set; }
        public String id_codice_art { get; set; }
        public String id_cond_prezzo { get; set; }
        public decimal val_condizione { get; set; }
    }

    [Table("da_sconti_articolo", Schema = "public")]
    public class da_sconti_articolo
    {
        [Key]
        public long id { get; set; }
        public String id_codice_art { get; set; }
        public String id_cond_prezzo { get; set; }
        public decimal val_condizione { get; set; }
    }

    [Table("da_listini_cliente", Schema = "public")]
    public class da_listini_cliente
    {
        [Key]
        public long id { get; set; }
        public String id_codice_art { get; set; }
        public String id_cliente { get; set; }
        public String id_cond_prezzo { get; set; }
        public decimal val_condizione { get; set; }
    }

    [Table("da_sconti_cliente_articolo", Schema = "public")]
    public class da_sconti_cliente_articolo
    {
        [Key]
        public long id { get; set; }
        public String id_codice_art { get; set; }
        public String id_cliente { get; set; }
        public String id_cond_prezzo { get; set; }
        public decimal val_condizione { get; set; }
    }



}
using fastOrderEntry.Helpers;
using fastOrderEntry.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.fw;

namespace fastOrderEntry.Controllers
{
    public class AccettazioneController : Controller
    {
        private const int REC_X_PAGINA = 15;

        private PetLineContext db = new PetLineContext();
        private NpgsqlConnection con = null;

        public AccettazioneController()
        {
            con = DbUtils.GetDefaultConnection();
        }

        public ActionResult Index()
        {
            ViewBag.page = "accettazione";
            return View();
        }

        public JsonResult GetPaginatore(AccettazioneFilters filters)
        {
            int count = 0;
            try
            {
                con.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = @"select count(*) as cnt from ao_accettazione 
                                    Where ao_accettazione.id_divisione='1'  ";

                    if (filters.esercizio.HasValue)
                        cmd.CommandText += $" and ao_accettazione.esercizio={filters.esercizio.Value}";

                    if (!string.IsNullOrEmpty(filters.id_accettazione))
                        cmd.CommandText += $" and ao_accettazione.id_accettazione = '{filters.id_accettazione.PadLeft(10, '0')}'";

                    if (!string.IsNullOrEmpty(filters.id_tipo_accettazione))
                        cmd.CommandText += $" and ao_accettazione.id_tipo_accettazione  = '{filters.id_tipo_accettazione}'";

                    if (!string.IsNullOrEmpty(filters.id_fornitore))
                        cmd.CommandText += $" and ao_accettazione.id_fornitore = '{filters.id_fornitore.PadLeft(10, '0')}'";

                    if (!string.IsNullOrEmpty(filters.data_accettazione))
                        cmd.CommandText += $" and ao_accettazione.data_accettazione = '{filters.data_accettazione}'";

                    cmd.ExecuteNonQuery();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader["cnt"]);
                        }
                    }
                }
                con.Close();
            }
            catch 
            {
                
            }

            JsonResult json = new JsonResult();
            json = Json(new { rec_number = count, rec_x_pagina = REC_X_PAGINA, pag_number = Math.Ceiling(1.0 * count / REC_X_PAGINA) }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        public JsonResult GetConenutoPagina(AccettazioneFilters filters)
        {
            List<AccettazioneRighe> model = new List<AccettazioneRighe>();
            try
            {
                con.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = @"select distinct ao_accettazione.*, aa_fornitori.ragione_sociale as rag_soc_for, 
                                    ac_accettazione_testa.descrizione as descr_tipo_acc  from ao_accettazione
                                    left join aa_fornitori on ao_accettazione.id_fornitore = aa_fornitori.id_fornitore 
                                    left join ac_accettazione_testa on ac_accettazione_testa.id_accettazione_testa = ao_accettazione.id_tipo_accettazione 
                                    Where ao_accettazione.id_divisione='1'  ";

                    if (filters.esercizio.HasValue)
                        cmd.CommandText += $" and ao_accettazione.esercizio={filters.esercizio.Value}";

                    if (!string.IsNullOrEmpty(filters.id_accettazione))
                        cmd.CommandText += $" and ao_accettazione.id_accettazione = '{filters.id_accettazione.PadLeft(10, '0')}'";

                    if (!string.IsNullOrEmpty(filters.id_tipo_accettazione))
                        cmd.CommandText += $" and ao_accettazione.id_tipo_accettazione  = '{filters.id_tipo_accettazione}'";

                    if (!string.IsNullOrEmpty(filters.id_fornitore))
                        cmd.CommandText += $" and ao_accettazione.id_fornitore = '{filters.id_fornitore.PadLeft(10, '0')}'";

                    if (!string.IsNullOrEmpty(filters.data_accettazione))
                        cmd.CommandText += $" and ao_accettazione.data_accettazione = '{filters.data_accettazione}'";

                    cmd.CommandText += $" order by ao_accettazione.esercizio desc limit {REC_X_PAGINA} offset {filters.page_number * REC_X_PAGINA}";

                    cmd.ExecuteNonQuery();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccettazioneRighe x = new AccettazioneRighe();
                            x.id_tipo_accettazione = Convert.ToString(reader["id_tipo_accettazione"]);
                            x.descr_tipo_acc = Convert.ToString(reader["descr_tipo_acc"]);
                            x.esercizio = Convert.ToInt16(reader["esercizio"]);
                            try { x.data_accettazione = Convert.ToDateTime(reader["data_accettazione"]).ToString("yyyy-MM-dd"); } catch { x.data_accettazione = null; }                            
                            x.id_accettazione = Convert.ToString(reader["id_accettazione"]);
                            x.id_fornitore = Convert.ToString(reader["id_fornitore"]);
                            x.rag_soc_for = Convert.ToString(reader["rag_soc_for"]);
                            x.imponibile = Convert.ToDecimal(reader["imponibile"]);
                            x.totale_doc = Convert.ToDecimal(reader["totale_doc"]);
                            x.id_divisa = Convert.ToString(reader["id_divisa"]);
                            x.rif_fornitore = Convert.ToString(reader["rif_fornitore"]);
                            try { x.data_rif_for = Convert.ToDateTime(reader["data_rif_for"]).ToString("yyyy-MM-dd"); } catch { x.data_rif_for = null; }                            
                            x.nr_documento = Convert.ToString(reader["nr_documento"]);
                            model.Add(x);
                        }
                    }

                }
                con.Close();
            }
            catch { }
            

            return Json(model,JsonRequestBehavior.AllowGet);
        }

        public JsonResult Select(AccettazioneRighe accettazione)
        {            
            List<AccettazioneDettaglioRighe> righe = new List<AccettazioneDettaglioRighe>();
            try
            {
                con.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = @"select  nr_riga,
                    (select s.id_cod_articolo from aa_source_list as s where s.id_divisione='1' and s.id_cod_articolo=ao_accettazione_righe.id_codice_art and s.id_fornitore='0000000080') as cod_art_fornitore,
                    id_codice_art, id_societa, id_partita, esercizio_ordine, id_ordine_acq, nr_riga_ord, nr_schedulazione, quantita, id_um, id_iva, prezzo_unitario,
                    imponibile,  imposta, str_sconto, id_det_prezzo, id_accettazione_riga, totale_riga, descrizione, id_magazzino, id_classe_valorizzazione, id_conto,
                    chiudi_riga, omaggio, zpet_controllato
                    from ao_accettazione_righe where id_divisione = '1' and esercizio= " + accettazione.esercizio + " and id_accettazione = '" + accettazione.id_accettazione + "'";

                    cmd.ExecuteNonQuery();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AccettazioneDettaglioRighe x = new AccettazioneDettaglioRighe();                            
                            x.nr_riga = Convert.ToInt32(reader["nr_riga"]);
                            x.cod_art_fornitore = Convert.ToString(reader["cod_art_fornitore"]);
                            x.id_codice_art = Convert.ToString(reader["id_codice_art"]);
                            x.id_societa = Convert.ToString(reader["id_societa"]);
                            x.id_partita = Convert.ToString(reader["id_partita"]);
                            x.esercizio_ordine = Convert.ToInt32(reader["esercizio_ordine"]);
                            x.id_ordine_acq = Convert.ToString(reader["id_ordine_acq"]);
                            x.nr_riga_ord = Convert.ToInt32(reader["nr_riga_ord"]);
                            x.nr_schedulazione = Convert.ToInt32(reader["nr_schedulazione"]);
                            x.quantita = Convert.ToDecimal(reader["quantita"]);
                            x.id_um = Convert.ToString(reader["id_um"]);
                            x.id_iva = Convert.ToString(reader["id_iva"]);                            
                            x.prezzo_unitario = Convert.ToDecimal(reader["prezzo_unitario"]);
                            x.imponibile = Convert.ToDecimal(reader["imponibile"]);
                            x.imposta = Convert.ToDecimal(reader["imposta"]);
                            x.str_sconto = Convert.ToString(reader["str_sconto"]);
                            try { x.id_det_prezzo = Convert.ToInt32(reader["id_det_prezzo"]); } catch { x.id_det_prezzo = null; }                            
                            x.id_accettazione_riga = Convert.ToString(reader["id_accettazione_riga"]);
                            x.totale_riga = Convert.ToDecimal(reader["totale_riga"]);
                            x.descrizione = Convert.ToString(reader["descrizione"]);
                            x.id_magazzino = Convert.ToString(reader["id_magazzino"]);
                            x.id_classe_valorizzazione = Convert.ToString(reader["id_classe_valorizzazione"]);
                            x.id_conto = Convert.ToString(reader["id_conto"]);
                            x.chiudi_riga = Convert.ToBoolean(reader["chiudi_riga"]);
                            x.omaggio = Convert.ToBoolean(reader["omaggio"]);
                            x.zpet_controllato = Convert.ToBoolean(reader["zpet_controllato"]);
                            righe.Add(x);
                        }
                    }
                }
                con.Close();

                AccettazioneDettaglio model = new AccettazioneDettaglio()
                {
                    accettazione = accettazione,
                    righe = righe
                };

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {   
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
       
    }

    public class AccettazioneFilters
    {
        public int page_number { get; set; }
        public int? esercizio { get; set; }
        public string id_tipo_accettazione { get; set; }
        public string id_fornitore { get; set; }
        public string data_accettazione { get; set; }
        public string id_accettazione { get; set; }
    }

    public class AccettazioneRighe
    {
        public string id_tipo_accettazione { get; set; }
        public string descr_tipo_acc { get; set; }
        public int esercizio { get; set; }
        public string data_accettazione { get; set; }
        public string id_accettazione { get; set; }
        public string cf { get; set; }
        public string id_fornitore { get; set; }
        public string rag_soc_for { get; set; }
        public decimal imponibile { get; set; }
        public decimal totale_doc { get; set; }
        public string id_divisa { get; set; }
        public string rif_fornitore { get; set; }
        public string data_rif_for { get; set; }
        public string nr_documento { get; set; }
    }

    public class AccettazioneDettaglioRighe
    {
        public int nr_riga { get; set; }
        public string cod_art_fornitore { get; set; }
        public string id_codice_art { get; set; }
        public string id_societa { get; set; }
        public string id_partita { get; set; }
        public int esercizio_ordine { get; set; }
        public string id_ordine_acq { get; set; }
        public int nr_riga_ord { get; set; }
        public int nr_schedulazione { get; set; }
        public decimal quantita { get; set; }
        public string id_um { get; set; }
        public string id_iva { get; set; }
        public decimal aliquota { get; set; }
        public decimal prezzo_unitario { get; set; }
        public decimal imponibile { get; set; }
        public decimal imposta { get; set; }
        public string str_sconto { get; set; }
        public long? id_det_prezzo { get; set; }
        public string id_accettazione_riga { get; set; }
        public decimal totale_riga { get; set; }
        public string descrizione { get; set; }
        public string id_magazzino { get; set; }
        public string id_classe_valorizzazione { get; set; }
        public string id_conto { get; set; }
        public bool chiudi_riga { get; set; }
        public bool omaggio { get; set; }
        public bool zpet_controllato { get; set; }
        public bool onEdit { get; set; }
    }

    public class AccettazioneDettaglio
    {
        public AccettazioneDettaglio()
        {
            righe = new List<AccettazioneDettaglioRighe>();
        }
        public virtual AccettazioneRighe accettazione { get; set; }
        public virtual IList<AccettazioneDettaglioRighe> righe { get; set; }
    }
}
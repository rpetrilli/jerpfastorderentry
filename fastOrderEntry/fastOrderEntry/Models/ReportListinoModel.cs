namespace fastOrderEntry.Models
{
    public class ReportListinoModel
    {
        public string articolo { get; set; }
        public string ean { get; set; }
        public string cod_fornitore { get; set; }
        public string descrizione { get; set; }
        public decimal prezzo_di_vendita { get; set; }
        public decimal giacenza { get; set; }
        public decimal sconto_1 { get; set; }
        public decimal sconto_2 { get; set; }
        public decimal sconto_3 { get; set; }
        public decimal prezzo_netto { get; set; }
        public string iva { get; set; }
        public decimal prezzo_di_acquisto { get; set; }
    }
}
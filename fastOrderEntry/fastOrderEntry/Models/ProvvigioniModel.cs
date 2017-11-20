using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    [Table("va_provvigioni_agente_articolo", Schema = "public")]
    public class ProvvigioniModel
    {
        public string id_societa { get; set; }
        public string id_agente { get; set; }
        public string id_codice_art { get; set; }
        public decimal val_provvigione { get; set; }
    }
}
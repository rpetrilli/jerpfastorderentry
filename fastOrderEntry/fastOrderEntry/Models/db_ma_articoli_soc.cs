using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fastOrderEntry.Models
{
    [Table("ma_articoli_soc", Schema = "public")]
    public class ma_articoli_soc
    {
        [Key]
        public string id_codice_art { get; set; }
        public string descrizione { get; set; }       
    }
}
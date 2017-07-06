using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fastOrderEntry.Models
{
    [Table("ca_divisa", Schema = "public")]
    public class ca_divisa
    {
        [Key]
        public string id_divisa { get; set; }
        public string descrizione { get; set; }
    }
}
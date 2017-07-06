using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
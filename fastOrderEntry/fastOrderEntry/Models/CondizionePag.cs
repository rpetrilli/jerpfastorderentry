using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    [Table("ca_cond_pag", Schema = "public")]
    public class CondizionePag
    {
        [Key]
        public string id_cond_pag { get; set; }
        public string descrizione { get; set; }

    }
}
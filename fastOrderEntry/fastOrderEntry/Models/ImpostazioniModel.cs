using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    [Table("zpet_impostazioni", Schema = "public")]
    public class Impostazioni
    {
        [Key]
        public long id { get; set; }
        public string jerp_url { get; set; }
        public string private_key { get; set; }
    }
}
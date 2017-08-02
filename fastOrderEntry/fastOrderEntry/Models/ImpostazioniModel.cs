using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    [Table("zpet_impostazioni")]
    public class Impostazioni
    {
        public string jerp_url { get; set; }
        public string private_key { get; set; }
    }
}
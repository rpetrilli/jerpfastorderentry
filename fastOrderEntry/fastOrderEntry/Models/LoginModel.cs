using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace fastOrderEntry.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string nomeUtente { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace fastOrderEntry.Models
{
    public class PetLineContext: DbContext
    {
        public PetLineContext() : base(nameOrConnectionString: "DefaultConnectionString") { }
        public virtual DbSet<ca_divisa> ca_divisa { get; set; }
    }
}
using fastOrderEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace fastOrderEntry.Controllers
{
    public class DivisaController : ApiController
    {
        // GET: api/Divisa
        public IEnumerable<ca_divisa> Get()
        {
            return new ca_divisa[] { new ca_divisa { id_divisa="EUR", descrizione="Euro" } };
        }

        // GET: api/Divisa/5
        public ca_divisa Get(int id)
        {
            return new ca_divisa { id_divisa="EUR", descrizione="Euro" };
        }

        // POST: api/Divisa
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Divisa/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Divisa/5
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace fastOrderEntry.Controllers
{
    public class OrdiniVenditaController : ApiController
    {
        // GET: api/OrdiniVendita
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/OrdiniVendita/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/OrdiniVendita
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/OrdiniVendita/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/OrdiniVendita/5
        public void Delete(int id)
        {
        }
    }
}

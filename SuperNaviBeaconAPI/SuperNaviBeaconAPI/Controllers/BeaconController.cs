using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SuperNaviBeaconAPI.Controllers
{
    public class BeaconController : ApiController
    {
        // GET: api/Beacon
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Beacon/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Beacon
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Beacon/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Beacon/5
        public void Delete(int id)
        {
        }
    }
}

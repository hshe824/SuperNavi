using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SuperNaviBeaconAPI.Controllers
{
    public class StoreLayoutController : ApiController
    {
        // GET api/StoreLayout
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/StoreLayout/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/StoreLayout
        public void Post([FromBody]string value)
        {
        }

        // PUT api/StoreLayout/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/StoreLayout/5
        public void Delete(int id)
        {
        }
    }
}
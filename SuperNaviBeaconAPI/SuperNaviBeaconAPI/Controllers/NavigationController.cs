using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SuperNaviBeaconAPI.Controllers
{
    public class NavigationController : ApiController
    {
        // GET api/Navigation
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/Navigation/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/Navigation
        public void Post([FromBody]string value)
        {
        }

        // PUT api/Navigation/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/Navigation/5
        public void Delete(int id)
        {
        }
    }
}
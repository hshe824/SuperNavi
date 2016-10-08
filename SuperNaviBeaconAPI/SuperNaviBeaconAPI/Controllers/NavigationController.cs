using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SuperNaviBeaconAPI.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace SuperNaviBeaconAPI.Controllers
{
    public class NavigationController : ApiController
    {
        private static Dictionary<String, Session> connections = new Dictionary<String, Session>();

        private CloudTable beaconTable = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudTableClient().GetTableReference("Beacon");

        // GET api/Navigation
        public IEnumerable<string> Get()
        {
            return new string[] {};
        }

        // GET api/Navigation/5
        public string Get(DtoBeacon beacon)
        {
            return "value";
        }

        // POST api/Navigation
        public String Post(List<DtoBeacon> beacons)
        {
            String ipAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            Session session;
            if (connections.ContainsKey(ipAddress))
            {
                session = connections[ipAddress];
            }
            else
            {
                session = new Session();
                connections[ipAddress] = session;
            }

            session.updateNewPosition(beacons);
            return session.getDirection();
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
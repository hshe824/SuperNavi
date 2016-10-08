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

        //Post api/Navigation
        public String Post(DtoBeacon beacon)
        {
            //Get the name of the supermarket with one of the beacons
            TableQuery<Beacon> query = new TableQuery<Beacon>();
            foreach(Beacon entity in beaconTable.ExecuteQuery(query))
            {
                if(entity.majorid == beacon.majorid &&
                   entity.minorid == beacon.minorid &&
                   entity.uuid == beacon.uuid)
                {
                    return entity.superMarket;
                }
            }
            return "Supermarket not found";
        }

        //POST api.Navigation
        public HttpStatusCode Post(List<DtoItem> items, String supermarketName)
        {
            String ipAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarketName));
            List<Beacon> allBeaconData = beaconTable.ExecuteQuery(query).ToList();

            Supermarket supermarket = new Supermarket()
            {
                name = supermarketName,
                allBeaconData = allBeaconData,
            };

            Session session = new Session()
            {
                supermarket = supermarket,
            };
            
            connections[ipAddress] = session;

            return HttpStatusCode.OK;
        }

        // POST api/Navigation
        public String Post(List<DtoBeacon> beacons)
        {
            String ipAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            Session session = connections[ipAddress];
            
            session.UpdateNewPosition(beacons);
            return session.GetDirection();
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
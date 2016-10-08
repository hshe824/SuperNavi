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
        //Maintains the session - One IP Address mapped to One Session
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
                    return entity.supermarket;
                }
            }
            return "Supermarket not found";
        }

        //POST api.Navigation
        /*
            Initialise the navigation

            Pass in the items that needs to be picked up as well as the supermarket name
            Session is created
        */
        public HttpStatusCode Post(List<DtoItem> items, String supermarketName)
        {
            //Get IP Address
            String ipAddress = Request.GetOwinContext().Request.RemoteIpAddress;

            //Get all beacon data for the supermarket
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarketName));
            List<Beacon> allBeaconData = beaconTable.ExecuteQuery(query).ToList();

            //Make the supermarket with the name and the beacon data
            Supermarket supermarket = new Supermarket()
            {
                name = supermarketName,
                allBeaconData = allBeaconData,
            };

            //Make the session
            Session session = new Session()
            {
                supermarket = supermarket,
            };
            
            //Store the session
            connections[ipAddress] = session;

            return HttpStatusCode.OK;
        }

        // POST api/Navigation
        /*
            Get the direction given the current position
        */
        public String Post(List<DtoBeacon> beacons)
        {
            String ipAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            //Retrieve session with IP Address
            Session session = connections[ipAddress];
            
            //Update position with the new client beacon data
            session.UpdateNewPosition(beacons);

            //Get direction to go to
            /*
                    Straight
                    Left
                    Right
            */
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
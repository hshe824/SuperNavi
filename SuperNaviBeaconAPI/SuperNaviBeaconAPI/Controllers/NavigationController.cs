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

        private CloudTable itemTable = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudTableClient().GetTableReference("Item");

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

        // GET api/Navigation/retrieved
        [Route("api/navigation/retrieved/{phoneID}")]
        [HttpGet]
        public String RetrievedItem(String phoneID)
        {
            Session session = connections[phoneID];

            String command = session.collectedItem();

            return command;
        }

        [Route("~/api/navigation/freeroam")]
        [HttpGet]
        public String Get() {

        }

        //Post api/Navigation
        [Route("~/api/navigation/")]
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
        [Route("~/api/navigation/{supermarketName}/{phoneID}")]
        public HttpStatusCode Post(DtoItemList list, String supermarketName, String phoneID)
        {
            //Get IP Address
            String ipAddress = phoneID;

            //Get all beacon data for the supermarket
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarketName));
            List<Beacon> allBeaconData = beaconTable.ExecuteQuery(query).ToList();


            //Make the supermarket with the name and the beacon data
            Supermarket supermarket = new Supermarket()
            {
                name = supermarketName,
                allBeaconData = allBeaconData,
                exit = new Models.Point() { X = 9, Y = 0},
            };

            supermarket.SetUp();

            //Getting all items from the supermarket
            List<Item> supermarketItems = new List<Item>();
            TableQuery<Item> queryItems = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket.name));
            foreach (Item item in itemTable.ExecuteQuery(queryItems)) {supermarketItems.Add(item);}

            //Make the session
            Session session = new Session(list.shoppingList, supermarket, supermarketItems);
            
            //Store the session
            connections[ipAddress] = session;

            return HttpStatusCode.OK;
        }

        // POST api/Navigation
        /*
            Get the direction given the current position
        */
        [Route("~/api/navigation/{phoneID}")]
        public String Post(DtoBeaconList list, String phoneID)
        {
            String ipAddress = phoneID;
            //Retrieve session with IP Address
            Session session = connections[ipAddress];
            
            //Update position with the new client beacon data
            session.UpdateNewPosition(list.beacons);

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

        // DELETE api/Navigation/
        [HttpDelete]
        public void Delete()
        {
        }

        // DELETE api/Navigation/reset
        [HttpDelete]
        [Route("api/navigation/reset")]
        public void Reset()
        {
            connections.Clear();
        }
    }
}
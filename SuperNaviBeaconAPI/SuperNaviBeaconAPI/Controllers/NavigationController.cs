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

        [Route("~/api/Navigation/freeroam/{phoneID}")]
        [HttpPost]
        public DtoString Freeroam(DtoBeaconList list, String phoneID)
        {
            if (!connections.Keys.Contains(phoneID)) {
                DtoItemList emptyList = new DtoItemList()
                {
                    shoppingList = new List<DtoItem>(),
                    beacon = list.beacons[0],
                };

                connections.Add(phoneID, generateSession(emptyList, phoneID));
            }

            Session retrieved = connections[phoneID];
            return new DtoString(retrieved.getNearbyItems(list));
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

        //POST api.Navigation
        /*
            Initialise the navigation

            Pass in the items that needs to be picked up as well as the supermarket name
            Session is created
        */
        [Route("~/api/navigation/item/{phoneID}")]
        public DtoItem Post(DtoItemList list, String phoneID)
        {
            
            //Store the session
            connections[phoneID] = generateSession(list, phoneID);

            return new DtoItem();
        }

        // POST api/Navigation
        /*
            Get the direction given the current position
        */
        [Route("~/api/navigation/{phoneID}")]
        public DtoString Post(DtoBeaconList list, String phoneID)
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
            String direction = session.GetDirection();
            return new DtoString(direction);
        }

        // DELETE api/Navigation/reset
        [HttpDelete]
        [Route("api/navigation/reset")]
        public void Reset()
        {
            connections.Clear();
        }

        private Session generateSession(DtoItemList list, String phoneID)
        {
            String supermarketName = "";

            //Get the name of the supermarket with one of the beacons
            TableQuery<Beacon> supermarketQuery = new TableQuery<Beacon>();
            foreach (Beacon entity in beaconTable.ExecuteQuery(supermarketQuery))
            {
                if (entity.majorid == list.beacon.majorid &&
                   entity.minorid == list.beacon.minorid &&
                   entity.uuid == list.beacon.uuid)
                {
                    supermarketName = entity.supermarket;
                }
            }

            //Get all beacon data for the supermarket
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarketName));
            List<Beacon> allBeaconData = beaconTable.ExecuteQuery(query).ToList();


            //Make the supermarket with the name and the beacon data
            Supermarket supermarket = new Supermarket()
            {
                name = supermarketName,
                allBeaconData = allBeaconData,
                exit = new Point() { X = 4, Y = 0 },
            };

            supermarket.SetUp();

            //Getting all items from the supermarket
            List<Item> supermarketItems = new List<Item>();
            TableQuery<Item> queryItems = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket.name));
            foreach (Item item in itemTable.ExecuteQuery(queryItems)) { supermarketItems.Add(item); }

            //Make the session
            return new Session(list.shoppingList, supermarket, supermarketItems);
        }
    }

}
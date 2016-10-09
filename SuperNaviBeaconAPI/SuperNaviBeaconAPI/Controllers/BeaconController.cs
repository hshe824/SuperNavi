using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SuperNaviBeaconAPI.Models;
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
        private CloudTable beaconTable = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudTableClient().GetTableReference("Beacon");
        
        // GET: api/Beacon
        [Route("~/api/beacon/{supermarket}")]
        [HttpGet]
        public IEnumerable<Beacon> Get(String supermarket)
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket));
            return beaconTable.ExecuteQuery(query).ToList();
        }

        // GET: api/Beacon/5
        [Route("~/api/beacon/{supermarket}/{id}")]
        [HttpGet]
        public IEnumerable<Beacon> Get(String supermarket, String id)
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            return beaconTable.ExecuteQuery(query).ToList();
        }

        // POST: api/Beacon
        [HttpPost]
        public IHttpActionResult Post(DtoBeacon dtoBeacon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Beacon beacon = dtoBeacon.toDomainObject();

            //See if the beacon exists
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, beacon.PartitionKey))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, beacon.RowKey));

            IEnumerable<Beacon> result = beaconTable.ExecuteQuery(query);
            Beacon beaconRetrieved = result.Count() > 0 ? result.First() : null;
            
            //If there was no beacon data for this position for this specific beacon, insert new
            if(beaconRetrieved == null)
            {
                beaconRetrieved = beacon;
            }
            //If there was
            else
            {
                //Get the averaged RSSI
                var rssi = beaconRetrieved.rssi;
                var totalrssi = rssi * beaconRetrieved.count;

                //Calculate the new average with the data that has just been recieved
                beaconRetrieved.rssi = (totalrssi + beacon.rssi) / (beaconRetrieved.count + 1);
                beaconRetrieved.count = beaconRetrieved.count + 1;
            }
            
            //Insert and update
            TableOperation insertOperation = TableOperation.InsertOrReplace(beaconRetrieved);
            
            beaconTable.Execute(insertOperation);

            return CreatedAtRoute("DefaultApi", new { id = beacon.uuid + beacon.majorid + beacon.minorid + beacon.positionX + beacon.positionY }, beacon.ToDto());
        }

        // PUT: api/Beacon/5
        [Route("~/api/beacon/{supermarket}/{id}")]
        [HttpPut]
        public IHttpActionResult Put(String supermarket, String id, DtoBeacon dtoBeacon)
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            TableOperation deleteOperation = TableOperation.Delete(beaconTable.ExecuteQuery(query).First());
            beaconTable.Execute(deleteOperation);
            Beacon beacon = dtoBeacon.toDomainObject();
            TableOperation insertOperation = TableOperation.Insert(beacon);
            beaconTable.Execute(insertOperation);
            return Ok(beacon);
        }

        // DELETE: api/Beacon/5
        [Route("~/api/beacon/{supermarket}/{id}")]
        [HttpDelete]
        public void Delete(String supermarket, String id)
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            TableOperation deleteOperation = TableOperation.Delete(beaconTable.ExecuteQuery(query).First());
            beaconTable.Execute(deleteOperation);
        }
    }
}

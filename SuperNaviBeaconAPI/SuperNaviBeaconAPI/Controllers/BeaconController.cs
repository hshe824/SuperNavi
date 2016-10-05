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
        [HttpGet]
        public IEnumerable<Beacon> Get()
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Beacon"));
            return beaconTable.ExecuteQuery(query).ToList();
        }

        // GET: api/Beacon/5
        public IEnumerable<Beacon> Get(String uuid)
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Beacon"))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, uuid));
            return beaconTable.ExecuteQuery(query).ToList();
        }

        // POST: api/Beacon
        public IHttpActionResult Post(DtoBeacon dtoBeacon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Beacon beacon = dtoBeacon.toDomainObject();
            TableOperation insertOperation = TableOperation.Insert(beacon);
            beaconTable.Execute(insertOperation);

            return CreatedAtRoute("DefaultApi", new { uuid = beacon.uuid }, beacon.ToDto());
        }

        // PUT: api/Beacon/5
        public IHttpActionResult Put(String uuid, DtoBeacon dtoBeacon)
        {
            Beacon beacon = dtoBeacon.toDomainObject();
            TableOperation updateOperation = TableOperation.InsertOrReplace(beacon);
            beaconTable.Execute(updateOperation);

            return CreatedAtRoute("DefaultApi", new { uuid = beacon.uuid }, beacon.ToDto());
        }

        // DELETE: api/Beacon/5
        public void Delete(String uuid)
        {
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Beacon"))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, uuid));
            TableOperation updateOperation = TableOperation.Delete(beaconTable.ExecuteQuery(query).First());
        }
    }
}

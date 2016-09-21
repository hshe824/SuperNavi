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
            beaconTable.CreateIfNotExists();
            TableQuery<Beacon> query = new TableQuery<Beacon>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Beacon"));
            return beaconTable.ExecuteQuery(query).ToList();
        }

        // GET: api/Beacon/5
        public string Get(int id)
        {
            return "value";
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

            return CreatedAtRoute("DefaultApi", new { id = beacon.id }, beacon.ToDto());
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

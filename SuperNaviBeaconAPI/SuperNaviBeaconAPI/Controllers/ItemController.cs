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
    public class ItemController : ApiController
    {
        private CloudTable itemTable = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudTableClient().GetTableReference("Item");

        // GET: api/Item
        [Route("~/api/item/{supermarket}")]
        [HttpGet]
        public IEnumerable<DtoItem> Get(String supermarket)
        {
            TableQuery<Item> query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Beacon"));
            List<DtoItem> dtoList = new List<DtoItem>();
            foreach (Item item in itemTable.ExecuteQuery(query).ToList())
            {
                dtoList.Add(item.ToDto());
            }
            return dtoList;
        }

        // GET: api/Item/5
        [Route("~/api/item/{supermarket}/{name}")]
        [HttpGet]
        public IEnumerable<DtoItem> Get(String supermarket, String name)
        {
            TableQuery<Item> query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Beacon"))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));
            List<DtoItem> dtoList = new List<DtoItem>();
            foreach (Item item in itemTable.ExecuteQuery(query).ToList())
            {
                dtoList.Add(item.ToDto());
            }
            return dtoList;
        }

        // POST: api/Item
        [HttpPost]
        public IHttpActionResult Post(DtoItem dtoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            itemTable.CreateIfNotExists();
            Item item = dtoItem.toDomainObject();
            TableOperation insertOperation = TableOperation.Insert(item);
            itemTable.Execute(insertOperation);

            return CreatedAtRoute("DefaultApi", new { id = item.name }, item.ToDto());
        }

        // PUT: api/Item/5
        [Route("~/api/item/{supermarket}/{name}")]
        [HttpPut]
        public IHttpActionResult Put(String supermarket, String name, DtoItem dtoItem)
        {
            TableQuery<Item> query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));
            TableOperation deleteOperation = TableOperation.Delete(itemTable.ExecuteQuery(query).First());
            itemTable.Execute(deleteOperation);
            Item item = dtoItem.toDomainObject();
            TableOperation insertOperation = TableOperation.Insert(item);
            itemTable.Execute(insertOperation);
            return Ok(item);
        }

        // DELETE: api/Item/5
        [Route("~/api/item/{supermarket}/{name}")]
        [HttpDelete]
        public void Delete(String supermarket, String name)
        {
            TableQuery<Item> query = new TableQuery<Item>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, supermarket))
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));
            TableOperation deleteOperation = TableOperation.Delete(itemTable.ExecuteQuery(query).First());
            itemTable.Execute(deleteOperation);
        }
    }
}

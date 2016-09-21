using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Beacon : TableEntity 
    {
        public Beacon(String id) {
            this.PartitionKey = "Beacon";
            this.RowKey = id;
        }
        public Beacon() { }
        public String id { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }

        public DtoBeacon ToDto()
        {
            return new DtoBeacon()
            {
                id = this.id,
                positionX = this.positionX,
                positionY = this.positionY
            };
        }
    }
}
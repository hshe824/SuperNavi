using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Beacon : TableEntity 
    {
        public Beacon(String ID) {
            this.PartitionKey = "Beacon";
            this.RowKey = ID;
        }
        public Beacon() { }
        public String ID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
    }
}
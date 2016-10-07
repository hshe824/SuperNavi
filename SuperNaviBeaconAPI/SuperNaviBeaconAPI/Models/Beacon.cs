using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Beacon : TableEntity 
    {
        public Beacon(String uuid, int majorid, int minorid) {
            this.PartitionKey = "Beacon";
            this.RowKey = uuid+majorid+minorid;
        }
        public Beacon() { }
        public String uuid { get; set; }
        public int majorid { get; set; }
        public int minorid { get; set; }
        public String rssi { get; set; }
        public String superMarket { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }

        public DtoBeacon ToDto()
        {
            return new DtoBeacon()
            {
                uuid = this.uuid,
                positionX = this.positionX,
                positionY = this.positionY,
                majorid = this.majorid,
                minorid = this.minorid,
                rssi = this.rssi,
                superMarket = this.superMarket
            };
        }
    }
}
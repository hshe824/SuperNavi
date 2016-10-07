using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Beacon : TableEntity 
    {
        public Beacon(String superMarket, String uuid, int majorid, int minorid, int positionX, int positionY) {
            this.PartitionKey = superMarket;
            this.RowKey = uuid+majorid+minorid+positionX+positionY;
            this.superMarket = superMarket;
            this.uuid = uuid;
            this.majorid = majorid;
            this.minorid = minorid;
            this.positionX = positionX;
            this.positionY = positionY;
        }
        public Beacon() { }
        [Required]
        public String uuid { get; set; }
        [Required]
        public int majorid { get; set; }
        [Required]
        public int minorid { get; set; }
        public String rssi { get; set; }
        public String superMarket { get; set; }
        [Required]
        public int positionX { get; set; }
        [Required]
        public int positionY { get; set; }
        public int count { get; set; }

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
                superMarket = this.superMarket,
                count = this.count
            };
        }
    }
}
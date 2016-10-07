using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class DtoBeacon
    {
        public DtoBeacon(String superMarket, String uuid, int majorid, int minorid)
        {
            this.superMarket = superMarket;
            this.uuid = uuid;
            this.majorid = majorid;
            this.minorid = minorid;
            this.positionX = positionX;
            this.positionY = positionY;
        }
        public DtoBeacon() { }

        public String uuid { get; set; }
        public int majorid { get; set; }
        public int minorid { get; set; }
        public int rssi { get; set; }
        public String superMarket { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int count { get; set; }

        public Beacon toDomainObject()
        {
            Beacon beacon = new Beacon(this.superMarket, this.uuid, this.majorid, this.minorid, this.positionX, this.positionY)
            {
                rssi = this.rssi,
                count = this.count
            };

            return beacon;
        }
    }
}
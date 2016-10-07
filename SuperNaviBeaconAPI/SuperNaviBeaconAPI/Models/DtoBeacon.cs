using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class DtoBeacon
    {
        public DtoBeacon(String uuid, int majorid, int minorid) {
            this.uuid = uuid;
            this.majorid = majorid;
            this.minorid = minorid;
        }
        public DtoBeacon() { }

        public String uuid { get; set; }
        public int majorid { get; set; }
        public int minorid { get; set; }
        public String rssi { get; set; }
        public String superMarket { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }

        public Beacon toDomainObject()
        {
            Beacon beacon = new Beacon(this.uuid, this.majorid, this.minorid)
            {
                positionX = this.positionX,
                positionY = this.positionY,
                rssi = this.rssi,
                superMarket = this.superMarket
            };

            return beacon;
        }
    }
}
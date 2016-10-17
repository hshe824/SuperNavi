using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class DtoBeacon
    {
        public String uuid { get; set; }
        public int majorid { get; set; }
        public int minorid { get; set; }
        public int rssi { get; set; }
        public String supermarket { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int count { get; set; }

        public Beacon toDomainObject()
        {
            Beacon beacon = new Beacon(this.supermarket, this.uuid, this.majorid, this.minorid, this.positionX, this.positionY)
            {
                rssi = this.rssi,
                count = this.count
            };

            return beacon;
        }
    }

    public class DtoBeaconList
    {
        public List<DtoBeacon> beacons { get; set; }
    }
}
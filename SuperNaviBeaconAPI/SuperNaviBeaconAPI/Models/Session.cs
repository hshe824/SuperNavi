using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperNaviBeaconAPI.Models;

namespace SuperNaviBeaconAPI.Models
{
    public class Session
    {
        public Supermarket supermarket { get; set; }
        private Beacon lastBeacon;

        internal void updateNewPosition(List<DtoBeacon> beacons)
        {
            throw new NotImplementedException();
        }

        internal String getDirection()
        {
            throw new NotImplementedException();
        }
    }
}
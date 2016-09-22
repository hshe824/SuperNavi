﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class DtoBeacon
    {
        public DtoBeacon(String id) {
            this.id = id;
        }
        public DtoBeacon() { }

        public String id { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }

        public Beacon toDomainObject()
        {
            Beacon beacon = new Beacon(this.id)
            {
                positionX = this.positionX,
                positionY = this.positionY,
            };

            return beacon;
        }
    }
}
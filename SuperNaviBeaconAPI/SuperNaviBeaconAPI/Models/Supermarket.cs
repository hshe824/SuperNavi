﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Supermarket
    {
        public String name { get; set; }
        public List<Beacon> allBeaconData { get; set; }
    }
}
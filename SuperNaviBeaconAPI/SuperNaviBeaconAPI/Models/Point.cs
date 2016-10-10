using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Score { get; set; }
        public Boolean walkable { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Supermarket
    {
        public String name { get; set; }
        public List<Beacon> allBeaconData { get; set; }
        private Dictionary<Point, List<Beacon>> map = new Dictionary<Point, List<Beacon>>();


        public void SetUp()
        {
            for(int x = 0; x <= 10; x++)
            {
                for(int y = 0; y <=10; y++)
                {
                    map[new Point()
                    {
                        X = x,
                        Y = y,
                    }] = new List<Beacon>();
                }
            }

            foreach(Beacon beacon in allBeaconData)
            {
                map[new Point()
                {
                    X = beacon.positionX,
                    Y = beacon.positionY,
                }].Add(beacon);
            }
        }

        public List<Beacon> GetBeaconDataAtPosition(int x, int y)
        {
            return map[new Point()
            {
                X = x,
                Y = y,
            }];
        }
    }
}
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

        public Point exit { get; set; }
        public Point entry { get; set; }

        /**
            Key: Point with X and Y
            Value: List of all beacon data for that Point
        */
        private Dictionary<Point, List<Beacon>> map = new Dictionary<Point, List<Beacon>>();

        public Supermarket() {}

        /**
            Setup all the points and the beacon data into the dictionary
        */
        public void SetUp()
        {
            for(int x = 0; x < 5; x++)
            {
                for(int y = 0; y < 5; y++)
                {
                    if (x % 2 != 0 || y == 0 || y == 4)
                        map[new Point()
                        {
                            X = x,
                            Y = y,
                            walkable = true,
                        }] = new List<Beacon>();

                    else {
                        map[new Point()
                        {
                            X = x,
                            Y = y,
                            walkable = false,
                        }] = new List<Beacon>();
                    }
                }
            }

            foreach(Beacon beacon in allBeaconData)
            {
                try {
                    map[new Point()
                    {
                        X = beacon.positionX,
                        Y = beacon.positionY,
                    }].Add(beacon);
                } catch(Exception e)
                {
                    var x = 1 + 1;
                }
            }
        }

        /**
            Retrieve all beacon data at that position
        */
        public List<Beacon> GetBeaconDataAtPosition(int x, int y)
        {
            return map[new Point()
            {
                X = x,
                Y = y,
            }];
        }

        public Boolean isWalkable(int x, int y)
        {
            Point temp = new Point() { X = x, Y = y };
            foreach (Point p in map.Keys)
            {
                if (p.Equals(temp))
                {
                    return p.walkable;
                }
            }
            return false;
        }
    }
}
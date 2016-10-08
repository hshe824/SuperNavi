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
        private List<Point> travelPath = new List<Point>();

        internal void UpdateNewPosition(List<DtoBeacon> beacons)
        {
            Point minimumDifferencePoint = new Point()
            {
                Score = int.MaxValue,
            };

            for(int x = 0; x <= 10; x++)
            {
                for(int y = 0; y <= 10; y++)
                {
                    List<Beacon> beaconsAtThatPosition = supermarket.GetBeaconDataAtPosition(x, y);

                    Dictionary<String, Beacon> beaconsAtThatPositionMap = new Dictionary<String, Beacon>();

                    foreach (Beacon beacon in beaconsAtThatPosition)
                    {
                        beaconsAtThatPositionMap[beacon.uuid + beacon.majorid + beacon.minorid] = beacon;
                    }

                    int localScore = 0;

                    foreach(DtoBeacon beaconChallenge in beacons)
                    {
                        String key = beaconChallenge.uuid + beaconChallenge.majorid + beaconChallenge.minorid;
                        if(beaconsAtThatPositionMap.ContainsKey(key))
                        {
                            Beacon beaconTemplate = beaconsAtThatPositionMap[key];
                            localScore += (int)(Math.Pow(beaconChallenge.rssi, 2) - Math.Pow(beaconTemplate.rssi, 2));
                        }
                        else
                        {
                            localScore += 30;
                        }
                    }

                    if(minimumDifferencePoint.Score > localScore)
                    {
                        minimumDifferencePoint = new Point()
                        {
                            X = x,
                            Y = y,
                            Score = localScore,
                        };
                    }
                }
            }
        }

        internal String GetDirection()
        {
            throw new NotImplementedException();
        }
    }
}
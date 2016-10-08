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

                    int localScore = 0;

                    foreach(DtoBeacon beaconChallenge in beacons)
                    {
                       foreach(Beacon beaconTemplate in beaconsAtThatPosition)
                       {
                            if(beaconChallenge.majorid == beaconTemplate.majorid &&
                                beaconChallenge.minorid == beaconTemplate.minorid &&
                                beaconChallenge.uuid == beaconTemplate.uuid)
                            {
                                localScore += Math.Abs(beaconChallenge.rssi - beaconTemplate.rssi);
                                break;
                            }
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
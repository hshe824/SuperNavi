using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperNaviBeaconAPI.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;

namespace SuperNaviBeaconAPI.Models
{
    public class Session
    {
        public Supermarket supermarket { get; set; }

        //the direction the user is facing
        private int Direction = 0;

        //Maintains a list of the points travelled
        private List<Point> travelPath = new List<Point>();

        public Dictionary<Point, Item> targets = new Dictionary<Point, Item>();

        private List<Item> groceryList = new List<Item>();

        private List<Item> supermarketItems = new List<Item>();

        private CloudTable itemTable = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudTableClient().GetTableReference("Item");


        /**
            Update the current position given the new beacon data
        */
        public Session(List<DtoItem> groc, Supermarket name, List<Item> supermarketItems)
        {
            this.supermarket = name;
            this.supermarketItems = supermarketItems;

            //Finding all the lists in 
            foreach(DtoItem dItem in groc) {
                foreach (Item item in this.supermarketItems) {
                    if (dItem.name.ToLower().Equals(item.name.ToLower())) {
                        groceryList.Add(item);
                    }
                }
            }

            orderGroceries();
            generateTargetPoints();
        }

        //Ordering groceries to be listed by the row, side and depth on the row
        private void orderGroceries() {
            var newList = groceryList.OrderBy(c => c.positionY).ThenBy(c => c.side).ThenBy(c => c.positionX);
            groceryList = newList.ToList();
        }

        //Generating the target points user needs to travel to
        private void generateTargetPoints() {
            foreach(Item item in groceryList) {
                int offset = 1;
                if (item.side == Item.Side.LEFT) {
                    offset = -1;
                }
                Point target = new Point()
                {
                    X = item.positionX + offset,
                    Y = item.positionY,
                };

                targets.Add(target, item);
            }
        }

        internal void UpdateNewPosition(List<DtoBeacon> beacons)
        {
            Point minimumDifferencePoint = new Point()
            {
                Score = int.MaxValue,
            };

            //Go through each X and Y to see which point has the smallest difference in RSSI value of beacons
            //Could be further optimised to start from the last point and spread outwards
            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    //Get all beacon data at the position
                    List<Beacon> beaconsAtThatPosition = supermarket.GetBeaconDataAtPosition(x, y);

                    //Map it so we dont have to for loop twice
                    Dictionary<String, Beacon> beaconsAtThatPositionMap = new Dictionary<String, Beacon>();

                    foreach (Beacon beacon in beaconsAtThatPosition)
                    {
                        beaconsAtThatPositionMap[beacon.uuid + beacon.majorid + beacon.minorid] = beacon;
                    }

                    int localScore = 0;

                    //For each beacon data client has just picked up
                    foreach(DtoBeacon beaconChallenge in beacons)
                    {
                        String key = beaconChallenge.uuid + beaconChallenge.majorid + beaconChallenge.minorid;
                        //If the template had the beacon data
                        if(beaconsAtThatPositionMap.ContainsKey(key))
                        {
                            //Calculate the difference using sum of squared differences
                            Beacon beaconTemplate = beaconsAtThatPositionMap[key];
                            localScore += (int)(Math.Pow(beaconChallenge.rssi, 2) - Math.Pow(beaconTemplate.rssi, 2));
                        }
                        else
                        {
                            //If template didnt have it, punish the point as it is unlikely that it is this position
                            localScore += 30;
                        }
                    }

                    //If the current score was less than the minimum so far, replace it
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

            travelPath.Add(minimumDifferencePoint);
            calcDirection();
        }

        internal String GetDirection()
        {
            throw new NotImplementedException();
        }

        //Calculating the direction user is facing from current point relative to next point
        private void calcDirection() {
            Point current = travelPath[travelPath.Count - 1];
            Point prev = travelPath[travelPath.Count - 2];

            if (current.X == prev.X && current.Y == prev.Y) {
                return;
            }

            if (current.Y > prev.Y)
            {
                this.Direction = 0;
            }
            else if (current.Y < prev.Y) {
                this.Direction = 180;
            }

            if (current.X > prev.X)
            {
                this.Direction = 90;
            }
            else if (current.X < prev.X) {
                this.Direction = 270;
            }
        }
    }
}
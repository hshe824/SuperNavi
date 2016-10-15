using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperNaviBeaconAPI.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using System.Text;

namespace SuperNaviBeaconAPI.Models
{
    public class Session
    {
        public Supermarket supermarket { get; set; }

        //the direction the user is facing
        private int Direction = 0;
        //Maintains a list of the points travelled
        private List<Point> travelPath = new List<Point>();

        //Mapping targets to points
        public Dictionary<Item, Point> targets = new Dictionary<Item, Point>();
        public Point currentTarget = new Point();
        public Boolean shoppingComplete = false;
        //List of grocery items from the user
        private List<Item> groceryList = new List<Item>();
        //List of all grocery items
        private List<Item> supermarketItems = new List<Item>();

        private CloudTable itemTable = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudTableClient().GetTableReference("Item");

        //Stores all intermediate points between current position and target
        private List<Point> intermediatePoints = new List<Point>();

        private Dictionary<int, string> relativeDirectionMap = new Dictionary<int, string>();

        private String prevCommand;
        private DateTime prevCommandTime = DateTime.Now;

        /**
            Update the current position given the new beacon data
        */
        public Session(List<DtoItem> groc, Supermarket name, List<Item> supermarketItems)
        {
            populateRelativeMap();
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

            if (groc.Count > 0) {
                orderGroceries();
                generateTargetPoints();
            }
        }

        //Use this to indicate the item was collected
        public String collectedItem() {
            StringBuilder command = new StringBuilder();
            groceryList.RemoveAt(0);
            if (groceryList.Count == 0)
            {
                currentTarget = supermarket.exit;
                command.Append("Now proceeding to checkout.");
                shoppingComplete = true;
            }
            else {
                currentTarget = targets[groceryList[0]];
                command.Append("Now collecting " + groceryList[0].name);                
                //ADD NEXT DIRECTION
            }
            prevCommandTime = DateTime.Now;
            command.Append(calculatePath(travelPath[travelPath.Count - 1], currentTarget));
            prevCommand = command.ToString();
            return command.ToString();
        }

        //Ordering groceries to be listed by the row, side and depth on the row
        private void orderGroceries() {
            var newList = groceryList.OrderBy(c => c.positionX).ThenByDescending(c => c.side).ThenBy(c => c.positionY);
            groceryList = newList.ToList();
        }

        //Generating the target points user needs to travel to
        private void generateTargetPoints() {
            foreach(Item item in groceryList) {
                int offset = 1;
                if (item.side.ToLower().Equals("left")) {
                    offset = -1;
                }
                Point temp = new Point()
                {
                    X = item.positionX + offset,
                    Y = item.positionY,
                };

                targets.Add(item, temp);
            }

            currentTarget = targets[groceryList[0]];
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
                            localScore += Math.Abs((int)(Math.Pow(beaconChallenge.rssi, 2) - Math.Pow(beaconTemplate.rssi, 2)));
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
        }

        internal String GetDirection()
        {
            //must be first poll, get user to walk to right
            if (travelPath.Count < 2)
            {
                prevCommandTime = DateTime.Now;
                return ("Welcome to " + supermarket.name + ". Proceed by walking to the right.");
            }

            Point current = travelPath[travelPath.Count - 1];

            //If target is the exit, thank user for using app
            if (shoppingComplete && current.Equals(currentTarget)) {
                return ("Thank you for using SuperNavi! Hope you enjoyed this service.");
            }


            StringBuilder command = new StringBuilder();

            int prevOrientation = this.Direction;

            calcDirection();

            //if at target alert them
            if (current.Equals(currentTarget)) {
                command.Append(groceryList[0].name + " is on the ");
                if ((groceryList[0].side.ToLower().Equals("left") && Direction == 0 ) || (groceryList[0].side.ToLower().Equals("right") && Direction == 180))
                {
                    command.Append("right. SIGNATURE");
                }
                else {
                    command.Append("left.SIGNATURE");
                }
                prevCommand = command.ToString();
                prevCommandTime = DateTime.Now;
                return command.ToString();
            }


            command.Append(calculatePath(current, currentTarget));
            TimeSpan diff =  DateTime.Now.Subtract(prevCommandTime);

            if (command.ToString().Equals(prevCommand) && prevOrientation == this.Direction && diff.TotalSeconds < 5) {
                return "SAME";
            }

            prevCommandTime = DateTime.Now;
            prevCommand = command.ToString();
            return command.ToString();
        }

        //Getting all the items right next to the current location of the user
        internal String getNearbyItems(DtoBeaconList list) {
            StringBuilder reply = new StringBuilder();
            UpdateNewPosition(list.beacons);

            Point currentPos = travelPath[travelPath.Count - 1];

            Boolean isLeft = false;
            //Getting all the items next to the users current position;
            foreach(Item i in supermarketItems){
                int offset = 1;
                if (i.side.ToLower().Equals("left")) {
                    offset = -1;
                    isLeft = true;
                }

                Point p = new Point()
                {
                    X = i.positionX + offset,
                    Y = i.positionY,
                };

                if (p.Equals(currentPos)) {
                    reply.Append(i.name + ", ");
                }
            }

            if (reply.ToString() == null || reply.ToString().Equals("")) {
                return "there are no items right next to you currently";
            }

            reply.Append("is on the ");
            if ((isLeft && Direction == 0) || (!isLeft && Direction == 180))
            {
                reply.Append("right.");
            }
            else
            {
                reply.Append("left.");
            }

            return reply.ToString();
        }

        //Calculating the direction user is facing from current point relative to next point
        private void calcDirection() {
            Point current = travelPath[travelPath.Count - 1];
            Point prev = travelPath[travelPath.Count - 2];

            if (current.Equals(prev)) {
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

        //Calculating the path to take next
        private string calculatePath(Point current, Point end)
        {
            int absDir = 0;
            //If X is the same, then just need to walk up or down
            if (current.X == end.X)
            {
                if (current.Y > end.Y)
                {
                    absDir = 180;
                }
                else {
                    absDir = 0;
                }
            }
            else
            {
                //walk left or right if no obstacle in the way
                if (current.X < currentTarget.X && supermarket.isWalkable(current.X + 1, current.Y))
                {
                    absDir = 90;
                }
                else if (current.X > currentTarget.X && supermarket.isWalkable(current.X - 1, current.Y))
                {
                    absDir = 270;
                }
                //Other wise walk to end of aisles
                else if ((current.Y) < 5)
                {
                    absDir = 180;
                }
                else
                {
                    absDir = 0;
                }
            }
           
            String command = relativeDirectionMap[(absDir - Direction)];
            return command;
        }

        //Populating map containing the relative direction to the associated command
        private void populateRelativeMap() {
            relativeDirectionMap.Add(0, "Keep Going Straight.");
            relativeDirectionMap.Add(90, "Turn Right.");
            relativeDirectionMap.Add(180, "Turn Around.");
            relativeDirectionMap.Add(270, "Turn Left.");
            relativeDirectionMap.Add(360, "Keep Going Straight.");
            relativeDirectionMap.Add(-90, "Turn Left.");
            relativeDirectionMap.Add(-180, "Turn Around.");
            relativeDirectionMap.Add(-270, "Turn Right.");
        }
    }
}
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class StoreLayoutNode : TableEntity
    {
        public StoreLayoutNode(int xCoordinate, int yCoordinate) {
            this.PartitionKey = "LayoutNode";
            this.RowKey = (xCoordinate + "," + yCoordinate);
            this.xCoordinate = xCoordinate;
            this.yCoordinate = yCoordinate;
            this.canWalk = true;
        }

        public int xCoordinate { get; }
        public int yCoordinate { get; }

        public Boolean canWalk { get; set; }
        
        public StoreLayoutNode northNode { get; set; }
        public StoreLayoutNode southNode { get; set; }
        public StoreLayoutNode eastNode { get; set; }
        public StoreLayoutNode westNode { get; set; }
    }
}
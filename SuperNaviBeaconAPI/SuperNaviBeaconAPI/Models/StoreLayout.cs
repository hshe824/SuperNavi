using Microsoft.WindowsAzure.Storage.Table;
using SuperNaviBeaconAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace superNaviBeaconAPI.Models
{
    public class StoreLayout : TableEntity

    {
        public StoreLayout(int width, int length) {
            this.width = width;
            this.length = length;

            this.layout = new StoreLayoutNode[width,length];
        }

        public int width { get; set; }
        public int length { get; set; }
        public StoreLayoutNode[,] layout { get; set; }

        public void setupLayout() {
            //Setting up Nodes
            for (int x = 0; x < this.layout.GetLength(0); x++) {
                for (int y = 0; y < this.layout.GetLength(1); y++) {
                    this.layout[x, y] = new StoreLayoutNode(x, y);
                }
            }




        }
    }
}
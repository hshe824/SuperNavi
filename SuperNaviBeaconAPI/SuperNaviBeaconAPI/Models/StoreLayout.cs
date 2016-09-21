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
            //LOGIC TO SET UP EACH NODE AND ITS RELATION GOES HERE


        }
    }
}
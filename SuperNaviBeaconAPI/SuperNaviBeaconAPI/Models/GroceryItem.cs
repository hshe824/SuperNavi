using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperNaviBeaconAPI.Models
{
    class GroceryItem
    {
        public String name { get; set; }
        public StoreLayoutNode node { get; set; }
        public int rackNumber { get; set; }

        public GroceryItem( String name, StoreLayoutNode node, rackNumber num) {
            this.name = name;
            this.node = node;
            this.rackNumber = num;
        }
    }
}

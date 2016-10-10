using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class DtoItem
    {
        public DtoItem() { }
        public String name { get; set; }
        public String supermarket { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public String side { get; set; }

        public Item toDomainObject()
        {
            Item item = new Item(this.supermarket, this.name)
            {
                positionX = this.positionX,
                positionY = this.positionY,
                side = (Item.Side) Enum.Parse(typeof(Item.Side),this.side,true),
            };

            return item;
        }
    }
}
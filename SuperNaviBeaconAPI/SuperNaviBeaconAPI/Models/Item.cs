using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class Item : TableEntity
    {
        public Item (String supermarket, String name)
        {
            this.PartitionKey = supermarket;
            this.RowKey = name;
            this.supermarket = supermarket;
            this.name = name;
        }

        public Item() { }
        [Required]
        public String name { get; set; }
        [Required]
        public String supermarket { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }

        public DtoItem ToDto()
        {
            return new DtoItem()
            {
                name = this.name,
                supermarket = this.supermarket,
                positionX = this.positionX,
                positionY = this.positionY,
            };
        }
    }
}
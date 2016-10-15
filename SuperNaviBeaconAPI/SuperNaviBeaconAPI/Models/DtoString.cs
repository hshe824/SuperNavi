using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperNaviBeaconAPI.Models
{
    public class DtoString
    {
        public DtoString(string str)
        {
            this.str = str;
        }

        String str { get; set; }
    }
}
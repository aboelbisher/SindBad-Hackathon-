using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace sindbad2.Models
{
    public class City
    {
        public string placeId {get ; set;}
        public Pair location { get; set; } // First = latitude , Second = longtitude
        public string name { get; set; }
    }
}
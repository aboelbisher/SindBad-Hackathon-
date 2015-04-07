using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sindbad2.Models
{
    public class Config
    {
        static public string appId = "AIzaSyBETqNbGV57K_URw4n-utr5asnfYjwZGYE";

        //static public String googleApi = "https://maps.googleapis.com/maps/api/place/details/";

        static public string searchNearByApi = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";
        /***********https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=-33.8670522,151.1957362&radius=500&types=food&name=cruise&key=AddYourOwnKeyHere *******/


        static public String getPlaceIdApi = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";
    }
}
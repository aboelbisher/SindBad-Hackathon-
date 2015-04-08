using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sindbad2.Models
{
    public class Config
    {
        static public string googleAppId = "AIzaSyBETqNbGV57K_URw4n-utr5asnfYjwZGYE";
        static public string amadeuisAppId = "dv9T2akqevMJ5GKHH5E8BSKsiNODLoY8";
        static public string expediaAppId = "44st9skw7nz2fs89fzw74yau";
        static public string expediaCCID = "55505";


        //static public String googleApi = "https://maps.googleapis.com/maps/api/place/details/";

        static public string searchNearByApi = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";
        static public string getPlaceIdApi = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";

        static public string getAirPortInfoApi = "http://api.sandbox.amadeus.com/v1.2/airports/nearest-relevant?";

        static public string getFlightsApi = "http://api.sandbox.amadeus.com/v1.2/flights/low-fare-search?";
        static public string getHotelApi = "http://dev.api.ean.com/ean-services/rs/hotel/v3/list?cid=55505";
    }
}
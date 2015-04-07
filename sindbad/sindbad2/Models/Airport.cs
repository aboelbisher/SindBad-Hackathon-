using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
 
namespace sindbad2.Models
{
    public class Airport
    {
        String IATA { get; set; }
        String name { get; set; }
        String cityIata { get; set; }
        String cityName { get; set; }
        double locationLat { get; set; }
        double locationLong { get; set; }
        public static Dictionary<string,Airport> makeAirport(JArray jsonArray)
        {
            Dictionary<string,Airport> retVal = new Dictionary<string,Airport>();
            for (int i=0;i<jsonArray.Count;i++)
            {

                string key = jsonArray[i]["airport"].ToString();
                retVal[key].IATA = key;
                retVal[key].cityIata = jsonArray[i]["DUB"].ToString();
                retVal[key].cityName = jsonArray[i]["city_name"].ToString();
                retVal[key].locationLat = Double.Parse(jsonArray[i]["location"]["latitude"].ToString());
                retVal[key].locationLong = Double.Parse(jsonArray[i]["location"]["longitude"].ToString());
                retVal[key].name = jsonArray[i]["airport_name"].ToString();
            }
            return retVal;
        }
    }
    public class Flight
    {
        String DeparteTime { get; set; }
        String ArrivingTime { get; set; }
        String departAirportName { get; set; }
        String ArrivingAirportName { get; set; }
        double price { get; set; }
    }
}
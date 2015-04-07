using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sindbad2.Models
{
    public class Airport
    {
        String IAta { get; set; }
        String name { get; set; }
        String cityIata { get; set; }
        String cityName { get; set; }
        double locationLat { get; set; }
        double locationLan { get; set; }

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
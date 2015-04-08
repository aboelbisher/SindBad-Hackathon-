﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace sindbad2.Models
{
    public class Airport
    {
        public String IATA { get; set; }
        public String name { get; set; }
        public String cityIata { get; set; }
        public String cityName { get; set; }
        public double locationLat { get; set; }
        public double locationLong { get; set; }
        public static Dictionary<string, Airport> makeAirport(JArray jsonArray)
        {
            Dictionary<string, Airport> retVal = new Dictionary<string, Airport>();
            for (int i = 0; i < jsonArray.Count; i++)
            {

                string key = jsonArray[i]["airport"].ToString();
                retVal[key] = new Airport();
                retVal[key].IATA = key;
                retVal[key].cityIata = jsonArray[i]["city"].ToString();
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
        public Airport from { get; set; }
        public Airport to { get; set; }
        public String DepartTime { get; set; }
        public String ArrivingTime { get; set; }

        public String flightNumber { get; set; }
        public String companyIATA { get; set; }
    }
    public class Trip
    {
        public Trip()
        {
            this.outBound = new List<Flight>();
            this.inBound = new List<Flight>();
        }
        public List<Flight> outBound { get; set; }
        public List<Flight> inBound { get; set; }
        public double price { get; set; }
        public static Trip MakeTrip(Dictionary<string, object> jsonArray, Dictionary<string, Airport> Airports)
        {
            Trip retVal = new Trip();
            var arr = jsonArray["results"] as JArray;
            if (arr.Count == 0)
            {
                return retVal;
            }
            var array = arr[0];
            retVal.price = Double.Parse(array["fare"]["total_price"].ToString());
            var array1 = array["itineraries"] as JArray;
            var array2 = array1[0];
            string[] collect = { "outbound", "inbound" };
            foreach (var iterator in collect)
            {
                foreach (var it in array2["outbound"]["flights"])
                {
                    Flight flight = new Flight();
                    flight.DepartTime = it["departs_at"].ToString();
                    flight.ArrivingTime = it["arrives_at"].ToString();
                    flight.flightNumber = it["flight_number"].ToString();
                    flight.companyIATA = it["marketing_airline"].ToString();
                    if (Airports.ContainsKey(it["origin"]["airport"].ToString()))
                    {
                        flight.from = Airports[it["origin"]["airport"].ToString()];
                    }
                    else
                    {
                        flight.from = new Airport();
                        flight.from.IATA = it["origin"]["airport"].ToString();
                        flight.from.cityIata = flight.from.IATA;
                        flight.from.name = flight.from.IATA;
                    }
                    if (Airports.ContainsKey(it["destination"]["airport"].ToString()))
                    {
                        flight.to = Airports[it["destination"]["airport"].ToString()];
                    }
                    else
                    {
                        flight.from = new Airport();
                        flight.from.IATA = it["origin"]["airport"].ToString();
                        flight.from.cityIata = flight.from.IATA;
                        flight.from.name = flight.from.IATA;
                    }
                    if (iterator.Equals("outbound"))
                        retVal.outBound.Add(flight);
                    else
                        retVal.inBound.Insert(0,flight);
                }
            }
            return retVal;
        }
    }
}
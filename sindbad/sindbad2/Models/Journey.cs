﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;


namespace sindbad2.Models
{

    #region ENUMS

    public enum GoogleAttractions
    {
        AMUSE_PARK, BAR, NIGHT_CLUB, CASINO

    };
    public static class GoogleAttractionsExtension
    {
        public static string discreption(this GoogleAttractions attraction)
        {
            switch (attraction)
            {
                case GoogleAttractions.AMUSE_PARK: return "amusement_park";
                case GoogleAttractions.BAR: return "bar";
                case GoogleAttractions.NIGHT_CLUB: return "night_club";
                case GoogleAttractions.CASINO: return "casino";



                default: throw new ArgumentOutOfRangeException("recipe category not supported");
            }
        }
        public static object attraction { get; set; }
    }

    public enum PRICE
    {
        ONE , TWO , THREE , FOUR  , FIVE , NONE//no price restrections
    }

    public enum TRAVEL_CLASS
    {
         FIRST, BUSINESS, PREMIUM_ECONOMY , ECONOMY
    }


    #endregion //ENUMS

    #region structs

    public struct Attraction
    {
        public Pair location;
        public string placeID;
        public string name;
        public double rating;
        public string vicinty; //area name
    }

    #endregion // structs

    public class Journey
    {

        private City fromCity;
        private City toCity;

        /*private string placeId;
        private Pair location; // First = latitude , Second = longtitude
        private string cityName;
         */ 
        private int maxPrice;
        private GoogleAttractions[] attractionsEnum;
        private int radius;
        private List<Attraction> attractions;
        private string startDate;
        private string endDate;
        private int adultsNum;
        private int childrenNum;
        private int infantsNum;
        private bool direct;
        private TRAVEL_CLASS travelClass;

        private Trip trip;

        private Dictionary<string, Airport> fromAirports;
        private Dictionary<string, Airport> toAirports;


        private Object thisLock = new Object();

        

        //private 
        //private string[] toIATA;


        /// <summary>
        /// city name ==> for example "Madrid" , or "Madrid , Spain"
        /// raduis = the distance's raduis you want to search 
        /// attractions = array of GoogleAttractions 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="raduis"></param>
        public Journey(string fromCityName, string toCityName , int maxPrice, GoogleAttractions[] attractions,string startDate ,
            string returnDate , int adultsNum , int childrenNum , int infantsNum , bool direct ,
            TRAVEL_CLASS travelClass , int raduis = 100)
        {

            this.startDate = startDate;
            this.endDate = returnDate;
            this.adultsNum = adultsNum;
            this.childrenNum = childrenNum;
            this.infantsNum = infantsNum;
            this.direct = direct;
            this.travelClass = travelClass;


            this.fromCity = new City { name = fromCityName };
            this.toCity = new City { name = toCityName };

            this.maxPrice = maxPrice;
            this.attractionsEnum = attractions;
            this.radius = raduis;

            this.getFromPlaceId(this.fromCity.name);

        }



        #region get place id
        private void getFromPlaceId(string name)
        {
            string apiUrl = Config.getPlaceIdApi + name + "&key=" + Config.googleAppId;
            SendBadHttpRequest.sendHttpRequest(apiUrl, getFromPlaceIdCompleted);

        }

        //saves the location and the place ID
        private void getFromPlaceIdCompleted(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp);
                var res = values["results"] as JArray;
                var placeId = res[0]["place_id"].ToString();
                var latitude = res[0]["geometry"]["location"]["lat"];
                var longtitude = res[0]["geometry"]["location"]["lng"];

                this.fromCity.location = new Pair(latitude.ToString(), longtitude.ToString());

                this.fromCity.placeId = placeId;
            }

            this.getToPlaceId(this.toCity.name);

        }

        private void getToPlaceId(string name)
        {
            string apiUrl = Config.getPlaceIdApi + name + "&key=" + Config.googleAppId;
            SendBadHttpRequest.sendHttpRequest(apiUrl, getToPlaceIdCompleted);

        }

        private void getToPlaceIdCompleted(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp);
                var res = values["results"] as JArray;
                var placeId = res[0]["place_id"].ToString();
                var latitude = res[0]["geometry"]["location"]["lat"];
                var longtitude = res[0]["geometry"]["location"]["lng"];

                this.toCity.location = new Pair(latitude.ToString(), longtitude.ToString());

                this.toCity.placeId = placeId;
            }

            this.getFromAirportsInfo(this.fromCity.location);

        }

        #endregion //get place id


        #region get Attractions

        private void getAttractions(int raduis , GoogleAttractions[] attractions , PRICE maxPrice) 
        {
            string locationApi = "location=" + this.toCity.location.First + "," + this.toCity.location.Second;
            string raduisApi = "&radius=" + raduis.ToString();
            string typesApi = this.makeAttractionApiString(attractions); // attractions
            //string minPriceApi = minPrice == PRICE.NONE ? "" : "&minprice=" + ((int)minPrice).ToString();
            string maxPriceApi = maxPrice == PRICE.NONE ? "" : "&maxprice=" + ((int)maxPrice).ToString();
            string appKeyApi = "&key=" + Config.googleAppId;

            string searcNearbyApi = Config.searchNearByApi + locationApi + raduisApi + typesApi + maxPriceApi + appKeyApi;

            SendBadHttpRequest.sendHttpRequest(searcNearbyApi, getAttractionsRequestCompleted);
        }

        private void getAttractionsRequestCompleted(IAsyncResult result)
        {
            
            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp);
                var res = values["results"] as JArray;

                List<Attraction> attractions = this.makeAttractionsFromString(res);

                this.attractions = attractions;


            }
            
        }

        private string makeAttractionApiString(GoogleAttractions[] attractions)
        {
            string types = "&types=";

            var count = attractions.Count();

            for (int i = 0; i < attractions.Count(); i++)
            {
                types += attractions[i].discreption();

                if (!(i == attractions.Count() - 1)) // not the first and not the last 
                {
                    types += "|";
                }
            }
            return types;
        }
        private List<Attraction> makeAttractionsFromString(JArray jsonArray)
        {
            List<Attraction> attractions = new List<Attraction>();

            for (int i = 0 ; i < jsonArray.Count ; i++)
            {
                string latitude = jsonArray[i]["geometry"]["location"]["lat"].ToString();
                string longtitude = jsonArray[i]["geometry"]["location"]["lng"].ToString();
                string placeId = jsonArray[i]["place_id"].ToString();
                string placeName = jsonArray[i]["name"].ToString();
                double? rating = (double?)jsonArray[i]["rating"];
                string vicinty = jsonArray[i]["vicinity"].ToString();

                Pair location = new Pair{First = latitude , Second = longtitude};

                Attraction attr = new Attraction { location = location,
                    name = placeName,
                    placeID = placeId,
                    rating = rating == null ? -1 : rating.Value,
                    vicinty = vicinty};

                attractions.Add(attr);
            }

            return attractions;
        }

        
        #endregion //get Attractions

        #region Aiport info 


        /// <summary>
        /// if from == true => get from airport info 
        /// if from == false => get to airport info 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="from"></param>
        private void getFromAirportsInfo(Pair location)
        {
            string apiUrl = Config.getAirPortInfoApi + "latitude="
                + location.First.ToString() + "&longitude=" + location.Second.ToString() + "&apikey=" + Config.amadeuisAppId;


            SendBadHttpRequest.sendHttpRequest(apiUrl, getFromAirportsInfoCompleted);


        }


        private void getFromAirportsInfoCompleted(IAsyncResult result)
        {

            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                JArray values = JsonConvert.DeserializeObject<JArray>(resp);

                this.fromAirports = Airport.makeAirport(values);

                var x = 0;

            }

            this.getToAirportsInfo(this.toCity.location);


        }

        private void getToAirportsInfo(Pair location)
        {
            string apiUrl = Config.getAirPortInfoApi + "latitude="
                + location.First.ToString() + "&longitude=" + location.Second.ToString() + "&apikey=" + Config.amadeuisAppId;


            SendBadHttpRequest.sendHttpRequest(apiUrl, getToAirportsInfoCompleted);


        }

        private void getToAirportsInfoCompleted(IAsyncResult result)
        {

            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                JArray values = JsonConvert.DeserializeObject<JArray>(resp);

                this.toAirports = Airport.makeAirport(values);             
            }

            
            foreach(KeyValuePair<string, Airport> fromAirPort in this.fromAirports)
            {
                foreach (KeyValuePair<string, Airport> toAirPort in this.toAirports)
                {
                    this.getFlights(fromAirPort.Value, toAirPort.Value, this.startDate, this.endDate
                        , this.adultsNum, this.childrenNum, this.infantsNum, this.direct, this.maxPrice, this.travelClass);
                }
            }

            var x = 0;


        }

        #endregion //Airport info 


        #region flights info 

        private void getFlights(Airport fromAirport , Airport toAirPort , string startDate , string returnDate ,
            int adultsNum , int childrenNum , int infantsNum , bool direct , int maxPrice , TRAVEL_CLASS travelClass)
        {
            string originApi = "origin=" + fromAirport.IATA;
            string dstApi = "&destination=" + toAirPort.IATA;
            string departureDateApi = "&departure_date=" + startDate;
            string retDateApi = "&return_date=" + returnDate;
            string adultsApi = "&adults=" + adultsNum.ToString();
            string childrenApi = "&children=" + childrenNum.ToString();
            string infantsApi = "&infants=" + infantsNum.ToString();
            string directApi = "&direct=false"; //+ direct.ToString().ToLower();
            string currencyApi = "&currency=USD";
            string maxPriceApi = "&max_price=" + maxPrice.ToString();
            string travelClassApi = "&travel_class=" + travelClass.ToString();
            string resultsNumApi = "&number_of_results=1";
            string appApiKey = "&apikey=" + Config.amadeuisAppId;

            string apiUrl = Config.getFlightsApi + originApi + dstApi + departureDateApi + retDateApi + adultsApi + childrenApi
                + infantsApi + directApi + currencyApi + maxPriceApi + travelClassApi + resultsNumApi+  appApiKey;

            SendBadHttpRequest.sendHttpRequest(apiUrl, getFlightsComplete);



        }

        private void getFlightsComplete(IAsyncResult result)
        {

            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp);

                lock (this.thisLock)
                {
                    if (this.trip == null)
                    {

                        this.trip = Trip.MakeTrip(values, this.fromAirports.Union(this.toAirports).ToDictionary(k => k.Key, v => v.Value));
                        //var result = d1.Union(d2).Union(d3).ToDictionary (k => k.Key, v => v.Value
                    }
                    else
                    {
                        Trip tmpTrip = Trip.MakeTrip(values, this.fromAirports.Union(this.toAirports).ToDictionary(k => k.Key, v => v.Value));
                        if (tmpTrip.price < this.trip.price)
                        {
                            this.trip = tmpTrip;
                        }
                    }
                }
            }

            var x = 0;

        }

        #endregion //flights info 
    }
}
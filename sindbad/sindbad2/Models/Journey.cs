﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Device.Location;
using System.Linq.Expressions;
using System.Data;


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
        ONE, TWO, THREE, FOUR, FIVE, NONE//no price restrections
    }

    public enum TRAVEL_CLASS
    {
        FIRST, BUSINESS, PREMIUM_ECONOMY, ECONOMY
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
        public string types;
    }

    #endregion // structs

    public class Journey
    {

        private City fromCity;
        private City toCity;
        private GeoCoordinate hotelLocation; // the nearest point to the attractions (MEDIAN)

        /*private string placeId;
        private Pair location; // First = latitude , Second = longtitude
        private string cityName;
         */
        public int maxPrice {get ; set;}
        public string attractionsString {get ; set;}
        public double radius {get ; set;}
        public List<Attraction> attractions {get ; set;}
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int adultsNum { get; set; }
        public int childrenNum { get; set; }
        public int infantsNum { get; set; }
        public bool direct { get; set; }
        public TRAVEL_CLASS travelClass { get; set; }
        public double minStarRate { get; set; }
        public List<Hotel> hotels { get; set; }
        public double remainMoney { get; set; }
        public bool ifCar { get; set; }
        public CarRent cars { get; set; }


        public Trip trip { get; set; }

        public Dictionary<string, Airport> fromAirports { get; set; }
        public Dictionary<string, Airport> toAirports { get; set; }

        public Dictionary<DateTime, List<Attraction>> randomisedAttractions;



        private Object thisLock = new Object();
        public bool finished = false;

        private int flightsNum; // for synchronizations




        /// <summary>
        /// city name ==> for example "Madrid" , or "Madrid , Spain"
        /// raduis = the distance's raduis you want to search 
        /// attractions = array of GoogleAttractions 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="raduis"></param>
        public Journey(string fromCityName, string toCityName, int maxPrice, string attractions, string startDate
            , string returnDate, int adultsNum, int childrenNum, int infantsNum, bool direct
            , TRAVEL_CLASS travelClass, bool ifCar ,double minStarRate = 0)
        {

            this.startDate = startDate;
            this.endDate = returnDate;
            this.adultsNum = adultsNum;
            this.childrenNum = childrenNum;
            this.infantsNum = infantsNum;
            this.direct = direct;
            this.travelClass = travelClass;
            this.minStarRate = minStarRate;
            this.remainMoney = maxPrice;
            this.ifCar = ifCar;


            this.fromCity = new City { name = fromCityName };
            this.toCity = new City { name = toCityName };

            this.maxPrice = maxPrice;
            this.attractionsString = attractions;
            //this.radius = raduis;

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

                GeoCoordinate northEastCoor = new GeoCoordinate(Double.Parse(res[0]["geometry"]["viewport"]["northeast"]["lat"].ToString()),
                   Double.Parse(res[0]["geometry"]["viewport"]["northeast"]["lng"].ToString()));

                GeoCoordinate southWestCoor = new GeoCoordinate(Double.Parse(res[0]["geometry"]["viewport"]["southwest"]["lat"].ToString()),
                   Double.Parse(res[0]["geometry"]["viewport"]["southwest"]["lng"].ToString()));

                GeoCoordinate centerCoor = new GeoCoordinate(Double.Parse(latitude.ToString()),
                   Double.Parse(longtitude.ToString()));

                double radius = Math.Max(centerCoor.GetDistanceTo(northEastCoor), centerCoor.GetDistanceTo(southWestCoor));
                this.radius = radius;
                this.toCity.location = new Pair(latitude.ToString(), longtitude.ToString());

                this.toCity.placeId = placeId;

                //this.getAttractions(radius, this.attractionsString);
            }

            this.getFromAirportsInfo(this.fromCity.location);

        }

        #endregion //get place id // first called


        #region get Attractions

        private void getAttractions(double raduis, string attractions)
        {
            string locationApi = "location=" + this.toCity.location.First + "," + this.toCity.location.Second;
            string raduisApi = "&radius=" + raduis.ToString();
            string typesApi = "&types=" + attractions; // attractions
            //string minPriceApi = minPrice == PRICE.NONE ? "" : "&minprice=" + ((int)minPrice).ToString();
            //string maxPriceApi = maxPrice == PRICE.NONE ? "" : "&maxprice=" + ((int)maxPrice).ToString();
            string appKeyApi = "&key=" + Config.googleAppId;

            string searcNearbyApi = Config.searchNearByApi + locationApi + raduisApi + typesApi + appKeyApi;

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

                this.getMedianOfAttractions();

                DateTime startDateTime = Convert.ToDateTime(this.trip.outBound.Last().ArrivingTime);
                DateTime endDateTime = Convert.ToDateTime(this.trip.inBound.Last().DepartTime);

                if(ifCar)
                {
                    string IATA = this.trip.outBound.Last().to.IATA;

                    this.getCar(IATA, startDateTime, endDateTime);  
                }

                string startDateString = startDateTime.Year.ToString() + "-" + startDateTime.Month.ToString()
                    + "-" + startDateTime.Day.ToString();
                this.getHotels(startDateString , this.endDate, this.hotelLocation, this.childrenNum, this.adultsNum, this.minStarRate, this.remainMoney);

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

            for (int i = 0; i < jsonArray.Count; i++)
            {
                string latitude = jsonArray[i]["geometry"]["location"]["lat"].ToString();
                string longtitude = jsonArray[i]["geometry"]["location"]["lng"].ToString();
                string placeId = jsonArray[i]["place_id"].ToString();
                string placeName = jsonArray[i]["name"].ToString();
                double? rating = (double?)jsonArray[i]["rating"];
                string vicinty = jsonArray[i]["vicinity"].ToString();
                string types = jsonArray[i]["types"].ToString();

                Pair location = new Pair { First = latitude, Second = longtitude };

                Attraction attr = new Attraction
                {
                    location = location,
                    name = placeName,
                    placeID = placeId,
                    rating = rating == null ? -1 : rating.Value,
                    vicinty = vicinty ,
                    types = types
                };

                attractions.Add(attr);
            }

            return attractions;
        }


        private void getMedianOfAttractions()
        {
            List<double> latitudes = new List<double>();
            List<double> longtitudes = new List<double>();

            foreach (var it in this.attractions)
            {
                latitudes.Add(Double.Parse(it.location.First.ToString()));
                longtitudes.Add(Double.Parse(it.location.Second.ToString()));
            }

            this.hotelLocation = new GeoCoordinate(latitudes.Median(), longtitudes.Median());

            var x = 0;


        }


        #endregion //get Attractions

        #region get car

        private void getCar(string IATA, DateTime startTime , DateTime endTime)
        {
            CarRent carRent = new CarRent(IATA, startTime, endTime);

            while(!carRent.finished){}

            this.cars = carRent;
        }

        private void getMaxCarPriceAndDecreaseMaxPrice(List<Car> cars)
        {
            double max = 0;
            foreach (Car car in cars)
            {
                if(car.estimatedTotal1 > max)
                {
                    max = car.estimatedTotal1;
                }
            }

            this.remainMoney -= max ;
        }


        #endregion // get car


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


            foreach (KeyValuePair<string, Airport> fromAirPort in this.fromAirports)
            {
                foreach (KeyValuePair<string, Airport> toAirPort in this.toAirports)
                {
                    this.getFlights(fromAirPort.Value, toAirPort.Value, this.startDate, this.endDate
                        , this.adultsNum, this.childrenNum, this.infantsNum, this.direct, this.maxPrice, this.travelClass);
                }
            }

            while(this.flightsNum < this.fromAirports.Count() * this.toAirports.Count()){}

            this.getAttractions(this.radius, this.attractionsString);


            var x = 0;


        }

        #endregion //Airport info // Second called


        #region flights info

        private void getFlights(Airport fromAirport, Airport toAirPort, string startDate, string returnDate,
            int adultsNum, int childrenNum, int infantsNum, bool direct, int maxPrice, TRAVEL_CLASS travelClass)
        {
            string originApi = "origin=" + fromAirport.IATA;
            string dstApi = "&destination=" + toAirPort.IATA;
            string departureDateApi = "&departure_date=" + startDate;
            string retDateApi = "&return_date=" + returnDate;
            string adultsApi = this.adultsNum > 0? "&adults=" + adultsNum.ToString() : "" ;
            string childrenApi = this.childrenNum > 0 ? "&children=" + childrenNum.ToString() : "";
            string infantsApi = this.infantsNum > 0 ? "&infants=" + infantsNum.ToString() : "";
            string directApi = "&direct=false"; //+ direct.ToString().ToLower();
            string currencyApi = "&currency=USD";
            string maxPriceApi = "&max_price=" + maxPrice.ToString();
            string travelClassApi = "&travel_class=" + travelClass.ToString();
            string resultsNumApi = "&number_of_results=1";
            string appApiKey = "&apikey=" + Config.amadeuisAppId;

            string apiUrl = Config.getFlightsApi + originApi + dstApi + departureDateApi + retDateApi + adultsApi + childrenApi
                + infantsApi + directApi + currencyApi + maxPriceApi + travelClassApi + resultsNumApi + appApiKey;

            SendBadHttpRequest.sendHttpRequest(apiUrl, getFlightsComplete);



        }

        private void getFlightsComplete(IAsyncResult result)
        {

            var request = (HttpWebRequest)result.AsyncState;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(result);
            }
            catch(Exception e)
            {
                lock(this.thisLock)
                {
                    this.flightsNum++;
                }
                return;
            }

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
                        this.remainMoney = this.maxPrice - this.trip.price;
                    }
                    else
                    {
                        Trip tmpTrip = Trip.MakeTrip(values, this.fromAirports.Union(this.toAirports).ToDictionary(k => k.Key, v => v.Value));
                        if (tmpTrip.price < this.trip.price)
                        {
                            this.trip = tmpTrip;
                            this.remainMoney = this.maxPrice - this.trip.price;
                        }
                    }

                    this.flightsNum++;
                }
            }
        }

        #endregion //flights info


        #region hotels

        private void getHotels(string startDate , string endDate , GeoCoordinate location , int childrenNum , int adultsNum , double minStarRate , double maxPrice)
        {


            DateTime startDT = Convert.ToDateTime(startDate);
            DateTime endDDT = Convert.ToDateTime(endDate);


           

            string apiKeyApi = "&apiKey=" + Config.expediaAppId;
            string minorRevApi = "&minorRev=4";
            string localeApi = "&locale=en_US";
            string currencyApi = "&currencyCode=USD";
            string latitudeApi = "&latitude=" + location.Latitude.ToString();
            string longtitudeApi = "&longitude=" + location.Longitude.ToString();
            string sortApi = "&sort=PROXIMITY";
            string arrivalDateApi = "&arrivalDate=" + startDT.Month.ToString() + "/" + startDT.Day.ToString()
                + "/" + startDT.Year.ToString() ;
            string departureDateApi = "&departureDate=" + endDDT.Month.ToString() + "/" + endDDT.Day.ToString()
                + "/" + endDDT.Year.ToString();
            string adultsNumApi = this.adultsNum > 0 ? "&Room.numberOfAdults=" + this.adultsNum.ToString() : "";
            string childrenNumApi = this.childrenNum > 0 ? "&Room.numberOfChildren=" + this.childrenNum.ToString() : "";
            string minStarRateApi = "&minStarRating=" + minStarRate.ToString();



            string requestString = Config.getHotelApi + apiKeyApi + minorRevApi + localeApi + currencyApi + latitudeApi + longtitudeApi +
                sortApi + arrivalDateApi + departureDateApi + adultsNumApi  + childrenNumApi + minStarRateApi;



            HttpWebRequest webRequest = WebRequest.Create(@requestString) as HttpWebRequest;
            webRequest.Timeout = 20000;
            webRequest.Method = "GET";
            webRequest.ContentType = "text/json; charset=UTF-8";
            webRequest.Accept = "application/json";
            webRequest.BeginGetResponse(new AsyncCallback(getHotelsCompleted), webRequest);




        }

        private void getHotelsCompleted(IAsyncResult result)
        {

            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                JToken values = JsonConvert.DeserializeObject<JToken>(resp);

                var obj = JObject.Parse(values.ToString());

                var dict = obj["HotelListResponse"].ToObject<Dictionary<string, object>>();
                this.hotels = Hotel.BestHotel(dict,this.remainMoney);
                this.remainMoney -= this.hotels.First().Price;
            }

            var randomiser = new DataToAttract();
            DateTime startDateTime = Convert.ToDateTime(this.trip.outBound.Last().ArrivingTime);
            DateTime endDateTime = Convert.ToDateTime(this.trip.inBound.Last().DepartTime);
            this.randomisedAttractions = randomiser.schedule(startDateTime, endDateTime, this.attractions);

            this.finished = true;

        #endregion// hotels

        }
    }
}
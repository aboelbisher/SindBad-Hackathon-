using Newtonsoft.Json;
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

        private string placeId;
        private Pair location; // First = latitude , Second = longtitude
        private string cityName;
        private PRICE minPrice ;
        private PRICE maxPrice;
        private GoogleAttractions[] attractionsEnum;
        private int radius;
        private List<Attraction> attractions;

        /// <summary>
        /// city name ==> for example "Madrid" , or "Madrid , Spain"
        /// raduis = the distance's raduis you want to search 
        /// attractions = array of GoogleAttractions 
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="raduis"></param>
        public Journey(string cityName, PRICE minPrice, PRICE maxPrice, GoogleAttractions[] attractions, int raduis = 100)
        {
            this.cityName = cityName;
            this.minPrice = minPrice;
            this.maxPrice = maxPrice;
            this.attractionsEnum = attractions;
            this.radius = raduis;

            this.getPlaceId(cityName);
        }



        #region get place id
        private void getPlaceId(string name)
        {
            string apiUrl = Config.getPlaceIdApi + name + "&key=" + Config.appId;
            SendBadHttpRequest.sendHttpRequest(apiUrl, getPlaceIdRequestCompleted);

        }

        //saves the location and the place ID
        private void getPlaceIdRequestCompleted(IAsyncResult result)
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

                this.location = new Pair(latitude.ToString(), longtitude.ToString());

                this.placeId = placeId;
            }

            this.getAttractions(this.radius, this.attractionsEnum, this.minPrice, this.maxPrice);
        }

        #endregion //get place id


        #region get Attractions

        private void getAttractions(int raduis , GoogleAttractions[] attractions , PRICE minPrice , PRICE maxPrice) 
        {
            string locationApi = "location=" + this.location.First + "," + this.location.Second;
            string raduisApi = "&radius=" + raduis.ToString();
            string typesApi = this.makeAttractionApiString(attractions); // attractions
            string minPriceApi = minPrice == PRICE.NONE ? "" : "&minprice=" + ((int)minPrice).ToString();
            string maxPriceApi = maxPrice == PRICE.NONE ? "" : "&maxprice=" + ((int)maxPrice).ToString();
            string appKeyApi = "&key=" + Config.appId;

            string searcNearbyApi = Config.searchNearByApi + locationApi + raduisApi + typesApi + minPriceApi + maxPriceApi + appKeyApi;

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
    }
}
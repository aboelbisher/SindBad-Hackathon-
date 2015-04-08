using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Device.Location;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace sindbad2.Models
{
    public class Hotel
    {
        public String name { set; get; }
        public String address { set; get; }
        public String city { set; get; }
        public double Rating { set; get; }
        public String ShortDesc { set; get; }
        public GeoCoordinate location { set; get; }
        public String hotelUrl { set; get; }
        public double Price { set; get;}
        public double tripAdvisorRating { set; get; }
        public String HotelIcon { set; get; }
        public static List<Hotel> BestHotel(Dictionary<string,object> jsonRetVal,double maxPrice)
        {
            List<Hotel> hotelsSorted = new List<Hotel>();
            Dictionary<Hotel, int> hotelHash = new Dictionary<Hotel, int>();
            string tmp = jsonRetVal["HotelList"].ToString();
            var tmpDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmp);
            var hotelLists = JsonConvert.DeserializeObject<JArray>(tmpDict["HotelSummary"].ToString());
            if(hotelLists.Count == 0)
            {
                throw new Exception("There is No Hotel");
            }
            for(int i=0;i<hotelLists.Count;i++)
            {
                Hotel hotel = new Hotel();
                hotel.address = hotelLists[i]["address1"].ToString();
                hotel.name = hotelLists[i]["name"].ToString();
                hotel.city = hotelLists[i]["city"].ToString();
                hotel.Rating = Double.Parse(hotelLists[i]["hotelRating"].ToString());
                hotel.ShortDesc = hotelLists[i]["shortDescription"].ToString();
                hotel.tripAdvisorRating = Double.Parse(hotelLists[i]["tripAdvisorRating"] == null ? "3.5" :
                                     hotelLists[i]["tripAdvisorRating"].ToString());
                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>
                    (hotelLists[i]["RoomRateDetailsList"].ToString());
                values = JsonConvert.DeserializeObject<Dictionary<string,object>>(values["RoomRateDetails"].ToString());
                values = JsonConvert.DeserializeObject<Dictionary<string,object>>(values["RateInfo"].ToString());
                values = JsonConvert.DeserializeObject<Dictionary<string,object>>(values["ChargeableRateInfo"].ToString());
                hotel.Price = Double.Parse(values["@total"].ToString());
                if(hotel.Price > maxPrice)
                {
                    continue;
                }
                hotel.location = new GeoCoordinate();
                hotel.location.Latitude = Double.Parse(hotelLists[i]["latitude"].ToString());
                hotel.location.Longitude = Double.Parse(hotelLists[i]["longitude"].ToString());
                hotel.hotelUrl =  hotelLists[i]["deepLink"].ToString();
                hotel.HotelIcon = "http://www.travelnow.com" + hotelLists[i]["thumbNailUrl"].ToString();
                hotelHash[hotel] = i;
                hotelsSorted.Add(hotel);
            }
            hotelsSorted = hotelsSorted.OrderBy(o => o.Price).ToList();
            for(int index =0 ; index < hotelsSorted.Count ; index++)
            {
                hotelHash[hotelsSorted[index]] += index;
            }
            hotelsSorted = hotelsSorted.OrderBy(o => o.tripAdvisorRating).ToList();
            for (int index = 0; index < hotelsSorted.Count; index++)
            {
                hotelHash[hotelsSorted[index]] += (hotelsSorted.Count - index);
            }
            List<Hotel> retVal = new List<Hotel>();
            for (int i = 0; i < 3; i++)
            {
                Hotel tmpMinHotel = null;
                int tmpMin = -1;
                foreach (KeyValuePair<Hotel, int> entry in hotelHash)
                {
                    if (tmpMinHotel == null || (tmpMin) > entry.Value)
                    {
                        tmpMinHotel = entry.Key;
                        tmpMin = entry.Value;
                    }
                }
                retVal.Add(tmpMinHotel);
                if (tmpMinHotel != null)
                {
                    hotelHash.Remove(tmpMinHotel);
                }
            }
            return retVal;
        }

    }
}
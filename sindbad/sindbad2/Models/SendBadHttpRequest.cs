using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace sindbad2.Models
{
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
    public class SendBadHttpRequest
    {


        public static void  getPlaceId(String name)
        {

            HttpWebRequest webRequest = WebRequest.Create(@"https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + name + "&key=" + Config.appId) as HttpWebRequest;
            webRequest.Timeout = 20000;
            webRequest.Method = "GET";
            webRequest.BeginGetResponse(new AsyncCallback(RequestCompleted), webRequest);

        }

        private static void RequestCompleted(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();

                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp);
                var res = values["results"] as 

                //var longtitude = res["location"];

                var x = 0;
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.UI;

namespace sindbad2.Models
{

    public class SendBadHttpRequest
    {


        public static void sendHttpRequest(string apiUrl , AsyncCallback requestCompleted )
        {
            HttpWebRequest webRequest = WebRequest.Create(@apiUrl) as HttpWebRequest;
            webRequest.Timeout = 20000;
            webRequest.Method = "GET";
            webRequest.BeginGetResponse(new AsyncCallback(requestCompleted), webRequest);

        }

      
        /*
        private static void RequestCompleted(IAsyncResult result)
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

                var x = 0;
            }

        }*/
    }
}
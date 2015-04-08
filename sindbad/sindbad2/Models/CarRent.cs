using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Security.Policy;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sindbad2.Models
{

    public class CarRent
    {
        private String IATA { get; set; }
        private DateTime pickUp { get; set; }
        private DateTime dropOff { get; set; }
        public List<Car> cars;
        private String apiUrl;
        public bool finished = false;


        public CarRent(String _IATA, DateTime _pickUp, DateTime _dropOff)
        {
            IATA = _IATA;
            pickUp = _pickUp;
            dropOff = _dropOff;
            String t1 = pickUp.Year.ToString() + "-" + pickUp.Month.ToString() + "-" + pickUp.Day.ToString();
            String t2 = dropOff.Year.ToString() + "-" + dropOff.Month.ToString() + "-" + dropOff.Day.ToString();

            apiUrl = "http://api.sandbox.amadeus.com/v1.2/cars/search-airport?location=" + IATA + "&pick_up=" + t1 + "&drop_off=" + t2 + "&apikey=dv9T2akqevMJ5GKHH5E8BSKsiNODLoY8";
            String t = apiUrl;
            SendBadHttpRequest.sendHttpRequest(apiUrl, FindCar);
        }

        private void FindCar(IAsyncResult result)
        {
            cars = new List<Car>();
            var request = (HttpWebRequest)result.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(result);
            using (var stream = response.GetResponseStream())
            {
                var r = new StreamReader(stream);
                var resp = r.ReadToEnd();
                Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp);
                var res = values["results"] as JArray;
                int count = 0;

                foreach (var _res in res)
                {
                    double latitude = double.Parse(_res["location"]["latitude"].ToString());
                    double longtitude = double.Parse(_res["location"]["longitude"].ToString());
                    KeyValuePair<double, double> location = new KeyValuePair<double, double>(latitude, longtitude);
                    String providerName = _res["provider"]["company_name"].ToString();
                    String line1 = _res["address"]["line1"].ToString();
                    String city = _res["address"]["city"].ToString();
                    var _cars = _res["cars"] as JArray;
                    foreach (var _car in _cars)
                    {
                        double price = double.Parse(_car["rates"][0]["price"]["amount"].ToString());
                        String _type = _car["rates"][0]["type"].ToString();
                        ratePlan type = getRatePlan(_type);
                        Url imageUrl = new Url(_car["images"][0]["url"].ToString());
                        double estimatedTotal = double.Parse(_car["estimated_total"]["amount"].ToString());
                        Car car = new Car(price, type, imageUrl, estimatedTotal, providerName, line1, city);
                        cars.Add(car);

                        ++count;
                        if (count == 5)
                        {
                            this.finished = true;
                            return;
                        }
                    }
                }
                this.finished = true;
            }
        }

        private static ratePlan getRatePlan(string _type)
        {
            switch (_type)
            {
                case "DAILY": return ratePlan.DAILY;
                case "WEEKEND": return ratePlan.WEEKEND;
                case "WEEKLY": return ratePlan.WEEKLY;
                case "MONTHLY": return ratePlan.MONTHLY;
                default: throw new RuntimeBinderException("NOT SUPPORTED PLAN");
            }
        }

    }
}
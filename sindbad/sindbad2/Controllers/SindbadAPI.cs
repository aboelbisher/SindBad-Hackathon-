using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using sindbad2.Models;

namespace sindbad2.API
{
    public class SindbadAPI : ApiController
    {

        public class SindbadPostClass
        {
            public string fromCityName { get; set; }
            public string toCityName { get; set; }
            public int maxPrice { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public int adultsNum { get; set; }
            public int childrenNum { get; set; }
            public int infantsNum { get; set; }
            public bool direct { get; set; }
            public TRAVEL_CLASS travelClass { get; set; }

            public string
                attractions { get; set; }
        }
        // POST: api/SindbadAPI
        public PostEnumerableResponse Post(SindbadPostClass post)
        {
            PostEnumerableResponse result = new PostEnumerableResponse
            {

                result = HttpPostResult.NOT_FOUND,
                data = null
            };

            Journey jr = new Journey(post.fromCityName, post.toCityName, post.maxPrice, post.attractions, post.startDate,
                post.endDate, post.adultsNum, post.childrenNum, post.infantsNum, post.direct, post.travelClass, myCallback);

            result.data = jr;
            return result;
        }

        public void myCallback(Journey jr)
        {
            var x = 0;
        }

    }

    public class PostEnumerableResponse : PostResponse
    {
        public Journey data { get; set; }
    }
    public enum HttpPostResult
    {
        UNAUTHORIZE = 0,
        NOT_FOUND = 1,
        NULL = 2,
        FOUND = 3,
        EMPTY = 4,
        WRONG_PASS = 5,
        SUCCESS = 6,
        FAIL = 7
    }
    public class PostResponse
    {
        public HttpPostResult result { get; set; }
    }
}

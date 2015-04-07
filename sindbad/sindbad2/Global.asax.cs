using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using sindbad2.Models;

namespace sindbad2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            CarRent t = new CarRent("SFO", new DateTime(2015, 4, 10), new DateTime(2015, 4, 20));
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

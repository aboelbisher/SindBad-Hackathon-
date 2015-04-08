using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using sindbad2.Models;
using System.Threading;

namespace sindbad2
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        public Object thisLock = new Object();
        public Journey journey;

        protected void Page_Load(object sender, EventArgs e)
        {

           // GoogleAttractions[] attractions = new GoogleAttractions[4] {GoogleAttractions.AMUSE_PARK , GoogleAttractions.BAR , GoogleAttractions.CASINO , GoogleAttractions.NIGHT_CLUB} ;
            Journey journey = new Journey("Paris", "London", 1000000, "food|zoo", "2015-07-25", "2015-07-30", 3, 2, 1, true, TRAVEL_CLASS.ECONOMY , true);

            Thread thread = new Thread(ThreadProc);

            thread.Start();
            thread.Join();


        }

        private static void ThreadProc()
        {
             

        }



    }
}
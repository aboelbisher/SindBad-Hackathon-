using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using sindbad2.Models;

namespace sindbad2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            GoogleAttractions[] attractions = new GoogleAttractions[4] {GoogleAttractions.AMUSE_PARK , GoogleAttractions.BAR , GoogleAttractions.CASINO , GoogleAttractions.NIGHT_CLUB} ;
            Journey journey = new Journey("Israel", "Paris", PRICE.NONE, attractions, 2000);

        }
    }
}
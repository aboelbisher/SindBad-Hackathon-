using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace sindbad2.Models
{
    public class DataToAttract
    {
        public Dictionary<DateTime, List<Attraction>> sched { get; set; }
        public List<Attraction> nightLife = new List<Attraction>();
        public List<Attraction> otherAttr = new List<Attraction>();
        public int jumpDist { get; set; }
        public static  double perc = 0.7;
        public void PrepareForSched(List<Attraction> attracts)
        {
            foreach (var iterator in attracts)
            {
                if(iterator.types.Contains("night") || iterator.types.Contains("bar")
                    || iterator.types.Contains("casino"))
                {
                    nightLife.Add(iterator);
                }
                else
                {
                    otherAttr.Add(iterator);
                }
            }
        }
        public Dictionary<DateTime, List<Attraction>> schedule(DateTime startDate, DateTime endDate, List<Attraction> attractions)
        {
            PrepareForSched(attractions);
            var attractionsPairs = getAttractionsInPairs();
            this.sched = new Dictionary<DateTime, List<Attraction>>();
            jumpDist = (int)((1 - perc) * (endDate.Subtract(startDate).Days));
            for(var dateIt = startDate.AddDays(1);dateIt.CompareTo(endDate) < 0;dateIt = dateIt.AddDays(jumpDist))
            {
                this.sched[dateIt] = new List<Attraction>();
                Random r = new Random();
                int indexToRemove = r.Next(otherAttr.Count);
                this.sched[dateIt].Add((Attraction)attractionsPairs[indexToRemove].First);
                this.sched[dateIt].Add((Attraction)attractionsPairs[indexToRemove].Second);
                attractionsPairs.RemoveAt(indexToRemove);
            }
            return sched;
          }

        public List<Pair> getAttractionsInPairs()
        {
            int i = 0;
            List<Pair> attractions = new List<Pair>();
            for (i = 0 ; i < this.nightLife.Count ; i++)
            {
                GeoCoordinate myCoor = new GeoCoordinate(Double.Parse(this.nightLife[i].location.First.ToString())
                    , Double.Parse(this.nightLife[i].location.Second.ToString()));
                int nearestIndex = this.getNearestAttr(myCoor);

                if(nearestIndex == -1)
                {
                    break;
                }

                Pair newPair = new Pair(this.nightLife[i] , this.otherAttr[nearestIndex]);

                attractions.Add(newPair);

                this.otherAttr.RemoveAt(nearestIndex);
            }
            
          
            if(!( i == this.nightLife.Count - 1))
            {
                  for(int j = i ; j < this.nightLife.Count ; j+=2)
                  {
                      Pair newPair = new Pair(this.nightLife[j] , this.nightLife[j+1]);
                      attractions.Add(newPair);
                  }
            }

            if(this.otherAttr.Count > 1)
            {
                for(int j = 0 ; j < this.otherAttr.Count ; j++)
                {
                    Pair newPair = new Pair(this.otherAttr[j], this.otherAttr[j + 1]);
                    attractions.Add(newPair);
                }

            }

            return attractions;

        }

        public int  getNearestAttr(GeoCoordinate cor)
        {
            double range = -1;
            int retVal = -1;
            for (int i = 0 ; i < this.otherAttr.Count ; i++) 
            {
                GeoCoordinate myCoor = new GeoCoordinate(Double.Parse(this.otherAttr[i].location.First.ToString())
                    , Double.Parse(this.otherAttr[i].location.Second.ToString()));

                if(range == -1 || range > cor.GetDistanceTo(myCoor))
                {
                    range = cor.GetDistanceTo(myCoor);
                    retVal = i;
                }
            }
            return retVal;
        }
    }
}
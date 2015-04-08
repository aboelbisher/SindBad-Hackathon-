using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public void 
            
            
            schedule(DateTime startDate,DateTime endDate,List<Attraction> attractions)
        {
            //var tmpList = 
            jumpDist = ((int)(1 - perc) * (endDate.Subtract(startDate).Days));
            for(var dateIt = startDate.AddDays(1);!dateIt.Equals(endDate);dateIt.AddDays(jumpDist))
            {
                this.sched[dateIt] = new List<Attraction>();
                foreach(Attraction it in attractions)
                {

                }
            }
        }
    }
}
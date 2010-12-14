using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenStreetApp
{
    public class Location
    {
        public double Longitude {get; set;}
        public double Latitude { get; set; }
        public String City { get; set; }
        public String Adress { get; set; }
        public String Area { get; set; }
        public String Description { get; set; }
        public String Line1 { get; set; }
        public String Line2 { get; set; }
        public String Line3 { get; set; }
        public String Line4 { get; set; }

        public override string ToString()
        {
            List<String> values = new List<String>{ City, Adress, Area, Description, Line1, Line2, Line3, Line4 };
            String returnString = "";
            bool isfirst = true;
            foreach (var s in values)
            {
                if (!String.IsNullOrEmpty(s) && !isfirst)
                {
                    returnString += ", " + s;
                }
                else if(!String.IsNullOrEmpty(s))
                {
                    returnString += s;
                    isfirst = false;
                }
            }
            return returnString;
        }
    }
}

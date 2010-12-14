using System;

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

        public override string ToString()
        {
            return "This is: " + Longitude + " and: " + Latitude;
        }
    }
}

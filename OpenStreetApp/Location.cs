using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenStreetApp
{
    [DataContract]
    public class Location
    {
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public String City { get; set; }
        [DataMember]
        public String Adress { get; set; }
        [DataMember]
        public String Area { get; set; }
        [DataMember]
        public String Description { get; set; }
        [DataMember]
        public String Line1 { get; set; }
        [DataMember]
        public String Line2 { get; set; }
        [DataMember]
        public String Line3 { get; set; }
        [DataMember]
        public String Line4 { get; set; }

        public override string ToString()
        {
            List<String> values = new List<String> { City, Adress, Area, Description, Line1, Line2, Line3, Line4 };
            String returnString = "";
            bool isfirst = true;
            foreach (var s in values)
            {
                if (!String.IsNullOrEmpty(s) && !isfirst)
                {
                    returnString += ", " + s;
                }
                else if (!String.IsNullOrEmpty(s))
                {
                    returnString += s;
                    isfirst = false;
                }
            }
            return returnString;
        }

        public String LocationListView
        {
            get
            {
                return this.ToString();
            }
        }
    }
}

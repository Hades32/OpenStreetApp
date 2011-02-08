using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace OpenStreetApp
{
    [DataContract]
    public class Location : INotifyPropertyChanged
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

        public String LocationListView
        {
            get
            {
                if (string.IsNullOrEmpty(this.Description))
                    return this.ToString();
                return this.Description;
            }
        }

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

        public override bool Equals(object obj)
        {
            var x = obj as Location;
            if (x == null)
                return false;

            int thisLat = (int)(this.Latitude * 10000);
            int thisLong = (int)(this.Longitude * 10000);
            int objLat = (int)(x.Latitude * 10000);
            int objLong = (int)(x.Longitude * 10000);

            return x.Adress == this.Adress
                && x.Area == this.Area
                && x.City == this.City
                && x.Description == this.Description
                && thisLat == objLat
                && x.Line1 == this.Line1
                && x.Line2 == this.Line2
                && x.Line3 == this.Line3
                && x.Line4 == this.Line4
                && thisLong == objLong;
        }

        public override int GetHashCode()
        {
            int thisLat = (int)(this.Latitude * 10000);
            int thisLong = (int)(this.Longitude * 10000);

            return this.Adress.TryGetHashCode()
                 ^ this.Area.TryGetHashCode()
                 ^ this.City.TryGetHashCode()
                 ^ this.Description.TryGetHashCode()
                 ^ thisLat.GetHashCode()
                 ^ this.Line1.TryGetHashCode()
                 ^ this.Line2.TryGetHashCode()
                 ^ this.Line3.TryGetHashCode()
                 ^ this.Line4.TryGetHashCode()
                 ^ thisLong.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

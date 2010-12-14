using System;
using System.Windows;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace OpenStreetApp
{
    public class OSMHelpers
    {
        public static Point WorldToTilePos(double lon, double lat, double zoom)
        {
            Point p = new Point();
            var zoomExp = Math.Pow(2.0, zoom);
            p.X = (float)((lon + 180.0) / 360.0 * zoomExp);
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * zoomExp);

            return p;
        }

        public static Point TileToWorldPos(double tile_x, double tile_y, double zoom)
        {
            Point p = new Point();
            var zoomExp = Math.Pow(2.0, zoom);
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / zoomExp);

            p.X = (float)((tile_x / zoomExp * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }

        public static void InputAdressToLocations(String inputAdressString, Action<List<Location>> callback)
        {
            List<Location> locations = new List<Location>();
            String encoded = System.Net.HttpUtility.UrlEncode(inputAdressString + ", *");
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                XDocument xdoc = XDocument.Parse(e.Result);
                XElement resultSetRoot =  xdoc.Element("ResultSet");
                
                var testresult = resultSetRoot.Elements("Result");
                
                //System.Diagnostics.Debug.WriteLine(testresult);

                foreach (XElement elem in testresult)
                {                
                    Location nl = new Location();
                    nl.Longitude = Double.Parse(elem.Element("longitude").Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                    nl.Latitude = Double.Parse(elem.Element("latitude").Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                    nl.Line1 = elem.Element("line1").Value;
                    nl.Line2 = elem.Element("line2").Value;
                    nl.Line3 = elem.Element("line3").Value;
                    nl.Line4 = elem.Element("line4").Value;
                // nl.City = elem.Element("city").Value;
                // nl.Adress = elem.Element("adress").Value;
                // nl.City = elem.Element("state").Value;
                    locations.Add(nl);               
                }
                callback(locations);
            };
            Uri adress = new Uri("http://where.yahooapis.com/geocode?q="
                + encoded + "&locale=" + System.Globalization.CultureInfo.CurrentCulture.Name + 
            "&appid=dj0yJmk9ZWMzSjkwU1JWOHE0JmQ9WVdrOVF6RlpRWFp5TjJzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZ");
            System.Diagnostics.Debug.WriteLine(adress);
            wc.DownloadStringAsync(adress);
        }
    }
}

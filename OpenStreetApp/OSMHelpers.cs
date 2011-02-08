using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace OpenStreetApp
{
    public static class OSMHelpers
    {

        public static void GeoPositionToLocation(Point geoCoordinate, Action<Location> callback)
        {
            var yCoord = geoCoordinate.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            var xCoord = geoCoordinate.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            Uri adress = new Uri("http://where.yahooapis.com/geocode?q=" + yCoord + ",+" + xCoord +
                            "&gflags=R&locale=" + System.Globalization.CultureInfo.CurrentCulture.Name +
                            "&appid=dj0yJmk9ZWMzSjkwU1JWOHE0JmQ9WVdrOVF6RlpRWFp5TjJzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZ");

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                XDocument xdoc = XDocument.Parse(e.Result);
                XElement resultSetRoot = xdoc.Element("ResultSet");

                var elem = resultSetRoot.Element("Result");

                Location location = new Location();
                location.Longitude = Double.Parse(elem.Element("longitude").Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                location.Latitude = Double.Parse(elem.Element("latitude").Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                location.Line1 = elem.Element("line1").Value;
                location.Line2 = elem.Element("line2").Value;
                location.Line3 = elem.Element("line3").Value;
                location.Line4 = elem.Element("line4").Value;

                System.Diagnostics.Debug.WriteLine(e.Result);

                callback(location);
            };
            wc.DownloadStringAsync(adress);
        }

        public static void InputAdressToLocations(String inputAdressString, Action<List<Location>> callback)
        {
            InputAdressAsync(inputAdressString, (List1) =>
            {
                InputAdressAsync(inputAdressString + ",*", (List2) =>
                {
                    callback(List1.Union(List2).Distinct().ToList());
                });
            });
        }

        private static void InputAdressAsync(String inputAdressString, Action<List<Location>> callback)
        {
            List<Location> locations = new List<Location>();
            String encoded = System.Net.HttpUtility.UrlEncode(inputAdressString);

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                XDocument xdoc = XDocument.Parse(e.Result);
                XElement resultSetRoot = xdoc.Element("ResultSet");

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
        public static IDictionary<string, string> parseQueryParameters(string query)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in query.Split('&'))
            {
                var tmp = item.Split('=');
                dict.Add(tmp[0], tmp[1]);
            }
            return dict;
        }

        public static string createQueryString(IDictionary<string, string> dict)
        {
            var sb = new System.Text.StringBuilder();
            bool first = true;
            foreach (var item in dict)
            {
                if (first == false)
                    sb.Append("&");
                else
                    first = false;

                sb.Append(item.Key);
                sb.Append("=");
                sb.Append(item.Value);
            }
            return sb.ToString();
        }

        public static void setOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = val;
            }
            else
            {
                dict.Add(key, val);
            }
        }

        public static TValue getOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue def)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return def;
            }
        }

        public static TValue getOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.getOrDefault(key, default(TValue));
        }

        /// <summary>
        /// Gets the HashCode or 0 if the object is NULL
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int TryGetHashCode(this object o)
        {
            if (o == null)
                return 0;
            else
                return o.GetHashCode();
        }
    }
}

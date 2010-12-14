﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace OpenStreetApp
{
    public static class OSMHelpers
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

        public static List<Location> InputAdressToLocations(String inputAdressString)
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
                    // nl.City = elem.Element("city").Value;
                    // nl.Adress = elem.Element("adress").Value;
                    // nl.City = elem.Element("state").Value;
                    locations.Add(nl);


                }
            };
            Uri adress = new Uri("http://where.yahooapis.com/geocode?q="
                + encoded + "&appid=dj0yJmk9ZWMzSjkwU1JWOHE0JmQ9WVdrOVF6RlpRWFp5TjJzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZ");
            System.Diagnostics.Debug.WriteLine(adress);
            wc.DownloadStringAsync(adress);
            return locations;
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
    }
}

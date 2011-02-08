using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace OpenStreetApp
{
    public static class CloudeMadeService
    {
        public static string Token { get; private set; }
        public static string UserId { get; private set; }
        public const string ApiKey = "1a8bcc813f9646519c9d2b12e92c69b2";

        private static System.Globalization.NumberFormatInfo icnformat
            = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

        static CloudeMadeService()
        {
            var r = new Random();
            UserId = r.Next(100000, 999999).ToString();
        }

        public static void authorize(Action callback)
        {
            // skip request if we already are authorized
            if (string.IsNullOrEmpty(Token) == false)
            {
                callback();
                return;
            }

            var uri = new Uri("http://auth.cloudmade.com/token/" + ApiKey + "?userid=" + UserId);
            var req = HttpWebRequest.CreateHttp(uri);
            req.Method = "POST";
            req.BeginGetResponse((a) =>
            {
                if (a.IsCompleted)
                {
                    var stream = req.EndGetResponse(a).GetResponseStream();
                    var buffer = new byte[512];
                    var len = stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    var result = System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, len);
                    if (string.IsNullOrEmpty(Token))
                        Token = result;
                    callback();
                }
            }, null);
        }

        public static void getRoute(GeoCoordinate start, GeoCoordinate end, IEnumerable<GeoCoordinate> vias,
                                    Action<IEnumerable<Waypoint>> callback)
        {
            string coordList = "";
            coordList += coordToString(start);
            if (vias != null && vias.FirstOrDefault() != null)
            {
                coordList += ",[";
                bool first = true;
                foreach (var via in vias)
                {
                    if (first == false)
                        coordList += ",";
                    else
                        first = false;
                    coordList += coordToString(via);
                }
                coordList += "]";
            }
            coordList += "," + coordToString(end);
            var url = "http://routes.cloudmade.com/" + ApiKey + "/api/0.3/" + coordList + "/car.gpx?lang=de&units=km&token=" + Token;
            var uri = new Uri(url);
            var req = HttpWebRequest.CreateHttp(uri);
            req.Method = "GET";
            req.BeginGetResponse((a) =>
            {
                if (a.IsCompleted)
                {
                    try
                    {
                        var stream = req.EndGetResponse(a).GetResponseStream();
                        var xml = XDocument.Load(stream);
                        stream.Close();
                        var route = parseRoute(xml);
                        callback(route);
                    }
                    catch (WebException e)
                    {
                        callback(null);
                    }
                    catch (Exception ex)
                    {
                        //TODO how to handle parsing errors?
                        throw new Exception("Unexpected data", ex);
                    }
                }
            }, null);
        }

        private static IEnumerable<Waypoint> parseRoute(XDocument xml)
        {
            var namedWaypoints = new List<Waypoint>();
            foreach (var wp in xml.Root.Element("{http://www.topografix.com/GPX/1/1}rte").Elements("{http://www.topografix.com/GPX/1/1}rtept"))
            {
                var coord = new GeoCoordinate()
                    {
                        Latitude = double.Parse(wp.Attribute("lat").Value, icnformat),
                        Longitude = double.Parse(wp.Attribute("lon").Value, icnformat)
                    };
                var info = wp.Element("{http://www.topografix.com/GPX/1/1}desc").Value;
                namedWaypoints.Add(new Waypoint()
                    {
                        Coordinate = coord,
                        Information = info
                    });
            }
            var wps = new List<Waypoint>();
            foreach (var wp in xml.Root.Elements("{http://www.topografix.com/GPX/1/1}wpt"))
            {
                var coord = new GeoCoordinate()
                {
                    Latitude = double.Parse(wp.Attribute("lat").Value, icnformat),
                    Longitude = double.Parse(wp.Attribute("lon").Value, icnformat)
                };
                var info = (from nwp in namedWaypoints
                            where nwp.Coordinate == coord
                            select nwp).FirstOrDefault();
                wps.Add(new Waypoint()
                {
                    Coordinate = coord,
                    Information = info == null ? null : info.Information
                });
            }
            return wps;
        }

        private static string coordToString(GeoCoordinate start)
        {
            return start.Latitude.ToString(icnformat) + "," + start.Longitude.ToString(icnformat);
        }
    }
}

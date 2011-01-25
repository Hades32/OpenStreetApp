using System.Device.Location;
using Microsoft.Phone.Controls.Maps;

namespace OpenStreetApp
{
    public static class RouteSimplifier
    {
        internal static LocationCollection simplifyRoute(LocationCollection points, double maxDistKM, LocationRect bounds)
        {
            var maxDist = maxDistKM * 1000;
            var res = new LocationCollection();

            double nsDeg = maxDistKM / 20000 * 90.0;
            double ewDeg = maxDistKM / 40000 * 180.0;

            bounds.South -= nsDeg;
            bounds.North += nsDeg;
            bounds.West -= ewDeg;
            bounds.East += ewDeg;

            var lastPos = -1;
            // get first point within rect
            for (int i = 0; i < points.Count; i++)
            {
                if (pointIsInRect(points[i], bounds))
                {
                    lastPos = i;
                    res.Add(points[i]);
                    break;
                }
            }

            if (lastPos == -1) 
                return res;

            //simplify route to have only segments of ~ maxDist
            for (int i = 1; i < points.Count; i++)
            {
                if (points[lastPos].GetDistanceTo(points[i]) > maxDist)
                {
                    if (i > lastPos + 1)
                    {
                        lastPos = i - 1;
                        res.Add(points[i - 1]);
                    }
                    else
                    {
                        lastPos = i;
                        res.Add(points[i]);
                    }
                }
            }

            //remove end of route that is outside of rect
            while (pointIsInRect(res[res.Count - 1], bounds) == false)
            {
                res.RemoveAt(res.Count - 1);
            }

            return res;
        }

        private static bool pointIsInRect(GeoCoordinate point, LocationRect bounds)
        {
            return point.Longitude >= bounds.West
                 && point.Longitude <= bounds.East
                 && point.Latitude <= bounds.North
                 && point.Latitude >= bounds.South;
        }
    }
}

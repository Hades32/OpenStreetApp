using System.Collections.Generic;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;

namespace OpenStreetApp
{
    public static class RouteSimplifier
    {
        private static LocationCollection lastRes, lastPoints;
        private static LocationRect lastBounds;

        internal static LocationCollection simplifyRoute(LocationCollection points, LocationRect bounds)
        {

            if (points == lastPoints && (bounds.East - bounds.West == lastBounds.East - lastBounds.West))
                return lastRes;

            var res = new LocationCollection();
            var res2 = new LocationCollection();

            int start = -1, end = -1;
            // get first point within rect
            for (int i = 0; i < points.Count; i++)
            {
                if (pointIsInRect(points[i], bounds))
                {
                    start = i;
                    break;
                }
            }

            if (start == -1)
                return res;

            // get last point within rect
            for (int i = points.Count - 1; i >= 0; i--)
            {
                if (pointIsInRect(points[i], bounds))
                {
                    end = i;
                    break;
                }
            }

            for (int i = start; i <= end; i++)
            {
                res.Add(points[i]);
            }

            List<int> keep = new List<int>();
            for (int i = 0; i < res.Count; i++)
            {
                keep.Add(i);
            }
            double[] distances = new double[res.Count];
            for (int i = 1; i < res.Count - 2; i++)
            {
                var distDelta = getDistanceDelta(res[i - 1], res[i], res[i + 1]);
                distances[i] = distDelta;
            }

            //simplify route to have only max 400 segments 
            while (keep.Count > 400)
            {
                int removeId = 0;
                double remDistance = double.MaxValue;
                for (int i = 1; i < keep.Count - 2; i++)
                {
                    if (distances[keep[i]] < remDistance)
                    {
                        removeId = i;
                        remDistance = distances[keep[i]];
                    }
                }
                if (removeId - 2 >= 0)
                    distances[keep[removeId - 1]] = getDistanceDelta(res[keep[removeId - 2]],
                                                res[keep[removeId - 1]],
                                                res[keep[removeId + 1]]);

                if (removeId + 2 < keep.Count)
                    distances[keep[removeId + 1]] = getDistanceDelta(res[keep[removeId - 1]],
                                                res[keep[removeId + 1]],
                                                res[keep[removeId + 2]]);                
                keep.RemoveAt(removeId);
            }

            foreach (var i in keep)
            {
                res2.Add(res[i]);
            }

            lastRes = res2;
            lastBounds = bounds;
            lastPoints = points;

            return res2;

        }

        private static double getDistanceDelta(GeoCoordinate p1, GeoCoordinate p2, GeoCoordinate p3)
        {
            var distDelta = p1.GetDistanceTo(p2) + p2.GetDistanceTo(p3)
                            - p2.GetDistanceTo(p3);
            return distDelta;
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

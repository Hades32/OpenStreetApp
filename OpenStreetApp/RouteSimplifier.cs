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
            if (points == lastPoints && (bounds == lastBounds || contains(bounds, lastBounds)))
                if (lastRes != null)
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

            // add only the middle points (that are within the bounding rect) into the result set
            for (int i = start; i <= end; i++)
            {
                res.Add(points[i]);
            }

            // create list of all indexes of the result that that shall be used
            //at the beginning these are simply ALL points
            List<int> keep = new List<int>();
            for (int i = 0; i < res.Count; i++)
            {
                keep.Add(i);
            }
            //create an array that holds the the following information for each point:
            // how much shorter would the route get, if the point would be removed
            double[] distances = new double[res.Count];
            for (int i = 1; i < res.Count - 2; i++)
            {
                var distDelta = getDistanceDelta(res[i - 1], res[i], res[i + 1]);
                distances[i] = distDelta;
            }

            //simplify route to have only max 400 segments 
            while (keep.Count > 400)
            {
                // search for the point that would alter the route in the LEAST, if removed, and
                // then remove that point from the list of valid points and recalculate
                // the distances of the surrounding points
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

            // create the final result set that only contains the points that were decided to be the most important ones
            foreach (var i in keep)
            {
                res2.Add(res[i]);
            }

            // cache results etc.
            lastRes = res2;
            lastBounds = bounds;
            lastPoints = points;

            return res2;

        }

        private static bool contains(LocationRect container, LocationRect inner)
        {
            return inner.West >= container.West
                && inner.East <= container.East
                && inner.North <= container.North
                && inner.South >= container.South;
        }

        private static double getDistanceDelta(GeoCoordinate p1, GeoCoordinate p2, GeoCoordinate p3)
        {
            var distDelta = p1.GetDistanceTo(p2) + p2.GetDistanceTo(p3)
                            - p2.GetDistanceTo(p3);
            return distDelta;
        }

        private static bool pointIsInRect(GeoCoordinate point, LocationRect bounds)
        {
            double diff = (bounds.East - bounds.West)/5;

            return point.Longitude >= (bounds.West - diff)
                 && point.Longitude <= (bounds.East + diff)
                 && point.Latitude <= (bounds.North + diff)
                 && point.Latitude >= (bounds.South - diff);
        }
    }
}

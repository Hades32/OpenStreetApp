using System;
using System.Windows;

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
    }
}

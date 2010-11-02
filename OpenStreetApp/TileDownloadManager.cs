using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace OpenStreetApp
{
    public class TileDownloadManager
    {
        private static TCollection<string> dictionary = new TCollection<string>();
        private static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        static TileDownloadManager()
        {
            //TODO: FIND SOLUTION FOR THREADS

            if (!isf.DirectoryExists("TileCache"))
            {
                isf.CreateDirectory("TileCache");
            }
        }

        /// <summary>
        /// This method downloads the image tile for the given coordinates and the given zoom level from the server 
        /// and then calls a callback function with a ImageSource created from the tile.
        /// this method is asynchronous
        /// </summary>
        /// <param name="coord">The world coordinates</param>
        /// <param name="zoom">The zoom level</param>
        /// <param name="callback">The callback function</param>
        public void fetch(Point coord, int zoom, Action<Point, double, string> callback)
        {

            var p = WorldToTilePos(coord.X, coord.Y, zoom);
            string tileName = p.X.ToString() + p.Y.ToString() + zoom.ToString();
            string path = Path.Combine("TileCache", tileName + ".png");

            if (!dictionary.containsSynchronized(tileName))
            {
                FileDownloader.download(
                                    new Uri("http://tile.openstreetmap.org/" + zoom + "/" + (int)p.X + "/" + p.Y + ".png"),
                                    path,
                                    (file) =>
                                    {
                                        dictionary.addSynchronized(tileName);
                                        callback(coord, zoom, path);
                                    });
            }
            else
                callback(coord, zoom, path);
        }

        public static Point WorldToTilePos(double lon, double lat, int zoom)
        {
            Point p = new Point();
            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        public static Point TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            Point p = new Point();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }
    }
}

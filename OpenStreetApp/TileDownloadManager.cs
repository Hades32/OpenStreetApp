using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;

namespace OpenStreetApp
{
    public class TileDownloadManager
    {
        private static TCollection<String> dictionary = new TCollection<String>();
        private static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        static TileDownloadManager()
        {
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
        public void fetch(Point coord, int zoom, Action<ImageSource> callback)
        {
            Thread t = new Thread(new ParameterizedThreadStart(fetch_async));
            tdm_task task;
            task.coord = coord;
            task.zoom = zoom;
            task.callback = callback;
            t.Start(task);
        }

        private void fetch_async(object obj)
        {
            var task = (tdm_task)obj;
            var p = WorldToTilePos(task.coord.X, task.coord.Y, task.zoom);
            BitmapSource bms;

            if (dictionary.containsSynchronized(p.X + "" + p.Y + "" + task.zoom))
            {
                bms = new BitmapImage(new Uri(Path.Combine("TileCache", p.X + "" + p.Y + "" + task.zoom + ".png")));
            }
            else
            {
                bms = new BitmapImage(new Uri("http://tile.openstreetmap.org/" + task.zoom + "/" + ((int)task.coord.X) + "/" + ((int)task.coord.Y) + ".png"));
                isf.CreateFile(Path.Combine("TileCache", p.X + "" + p.Y + "" + task.zoom + ".png"));
                dictionary.addSynchronized(p.X + "" + p.Y + "" + task.zoom);
            }
            task.callback(bms);
        }

        public Point WorldToTilePos(double lon, double lat, int zoom)
        {
            Point p = new Point();
            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        public Point TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            Point p = new Point();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }

        private struct tdm_task
        {
            public Point coord;
            public int zoom;
            public Action<ImageSource> callback;
        }
    }
}

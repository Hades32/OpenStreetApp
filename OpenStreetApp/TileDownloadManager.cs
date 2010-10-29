using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace OpenStreetApp
{
    class TileDownloadManager
    {
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
        }

        private struct tdm_task
        {
            public Point coord;
            public int zoom;
            public Action<ImageSource> callback;
        }
    }
}

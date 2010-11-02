using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OpenStreetApp
{
    public class GeoImage
    {
        #region zoom
        private double _zoom;
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (this._zoom != value)
                {
                    this._zoom = value;
                    this.OnZoomChanged();
                }
            }
        }
        private void OnZoomChanged()
        {
            updateImage();
        }
        #endregion

        #region Coordinate
        private Point _coord;
        public Point Coordinate
        {
            get { return _coord; }
            set
            {
                if (this._coord != value)
                {
                    this._coord = value;
                    this.OnCoordinateChanged();
                }
            }
        }
        private void OnCoordinateChanged()
        {
            updateImage();
        }
        #endregion

        public readonly Image Image = new Image();

        private TileDownloadManager tdm;

        public GeoImage(TileDownloadManager tdm)
        {
            this.Image.Width = 256;
            this.Image.Height = 256;
            this.tdm = tdm;
        }

        private void updateImage()
        {
            this.tdm.fetch(this.Coordinate, (int)this.Zoom,

                (coord, zoom, path) =>
                {   //as this happens async it might happen that this GeoImage has changed
                    //when the download has finished
                    if (coord == this.Coordinate && zoom == this.Zoom)
                        this.Image.Source = new BitmapImage(new Uri(path));
                });
        }

        public double MapHeight
        {
            get { return 90.0 / Math.Pow(2.0, this.Zoom); }
        }

        public double MapWidth
        {
            get { return 180.0 / Math.Pow(2.0, this.Zoom); }
        }
    }
}

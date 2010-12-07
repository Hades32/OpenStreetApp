using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Microsoft.Phone.Reactive;

namespace OpenStreetApp
{
    public partial class MapControl : UserControl
    {
        Point lastMouseLogicalPos = new Point();
        Point lastMouseViewPort = new Point();
        Point lastOSMPoint = new Point();
        double zoom = 1;
        double zoomCount = 1.0;
        double fixX = 0, fixY = 0;

        #region Source

        /// <summary>
        /// Source Dependency Property
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(MultiScaleTileSource), typeof(MapControl),
                new PropertyMetadata((MultiScaleTileSource)null,
                    new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// Gets or sets the Source property. This dependency property 
        /// indicates the currently used TileSource for the map.
        /// </summary>
        public MultiScaleTileSource Source
        {
            get { return (MultiScaleTileSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Source property.
        /// </summary>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl target = (MapControl)d;
            MultiScaleTileSource oldValue = (MultiScaleTileSource)e.OldValue;
            MultiScaleTileSource newValue = target.Source;
            target.OnSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Source property.
        /// </summary>
        protected virtual void OnSourceChanged(MultiScaleTileSource oldValue, MultiScaleTileSource newValue)
        {
            if (oldValue != null && newValue != null)
            {
                var oldpos = this.getCurrentPosition();
                var oldzoom = this.CurrentZoom;
                this.OSM_Map.Source = newValue;
                this.navigateToCoordinate(oldpos, oldzoom);
            }
            else
                this.OSM_Map.Source = newValue;
        }

        #endregion

        public double CurrentZoom
        {
            get
            {
                return Math.Log((1.0 / this.OSM_Map.ViewportWidth), 2);
            }
        }

        public MapControl()
        {
            InitializeComponent();

            // Implement double click
            Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.OSM_Map, "MouseLeftButtonUp")
            .BufferWithTimeOrCount(TimeSpan.FromSeconds(0.5), 2)
            .Subscribe(new Action<IList<IEvent<MouseButtonEventArgs>>>(
                eventList =>
                {
                    if (eventList.Count >= 2)
                        // subscribing directly on that dispatcher didn't work...
                        this.Dispatcher.BeginInvoke(OSM_Map_OnDoubleClick);
                }));

            this.OSM_Map.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OSM_Map_ManipulationDelta);
            this.OSM_Map.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(OSM_Map_ManipulationStarted);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            fixX = (this.OSM_Map.ActualWidth / 256.0) / 4.0;
            fixY = (this.OSM_Map.ActualHeight / 256.0) / 4.0;
        }

        public void navigateToInputAdress(String inputAdressString)
        {
            String encoded = System.Net.HttpUtility.UrlEncode(inputAdressString);
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                IEnumerable<XElement> locations = null;
                XElement resultSetRoot;
                if (!(e.Result.Length == 0))
                {
                    XDocument xdoc = XDocument.Parse(e.Result);
                    resultSetRoot = xdoc.Element("ResultSet");
                    locations = resultSetRoot.Elements("Result");
                }

                //// TEST CODE
                if (!(locations.Count() > 1))
                {
                    System.Diagnostics.Debug.WriteLine(locations.ElementAt(0).Element("latitude").ToString());
                    double x = Double.Parse(locations.ElementAt(0).Element("latitude").Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                    double y = Double.Parse(locations.ElementAt(0).Element("longitude").Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        navigateToCoordinate(new System.Device.Location.GeoCoordinate(x, y), 12);
                    });
                }
                else
                {
                    //TODO ERROR HANDLING
                    System.Diagnostics.Debugger.Break();
                }

            };
            Uri adress = new Uri("http://where.yahooapis.com/geocode?q="
                + encoded + "&appid=dj0yJmk9ZWMzSjkwU1JWOHE0JmQ9WVdrOVF6RlpRWFp5TjJzbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZ");
            System.Diagnostics.Debug.WriteLine(adress);
            wc.DownloadStringAsync(adress);
        }

        private void OSM_Map_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.lastMouseLogicalPos = e.GetPosition(this.OSM_Map);
            this.lastMouseViewPort = this.OSM_Map.ViewportOrigin;
            this.lastOSMPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
        }

        private void OSM_Map_OnDoubleClick()
        {
            var newzoom = zoom / 2.0;
            zoomCount *= 2;
            Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
            this.OSM_Map.ZoomAboutLogicalPoint(zoom / newzoom, logicalPoint.X, logicalPoint.Y);
            zoom = newzoom;
        }

        void OSM_Map_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.lastMouseLogicalPos = e.ManipulationOrigin;
            this.lastMouseViewPort = this.OSM_Map.ViewportOrigin;
            this.lastOSMPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
        }

        //Multi-Touch working
        void OSM_Map_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // Zoom
            if (e.DeltaManipulation.Scale.X != 0 || e.DeltaManipulation.Scale.Y != 0)
            {
                // zoom by average of X and Y scaling
                var zoom = Math.Max(Math.Abs(e.DeltaManipulation.Scale.X), Math.Abs(e.DeltaManipulation.Scale.Y));

                // ignore out-zooming when already zoomed out
                if (zoom < 1 && this.OSM_Map.ViewportWidth == 1 || zoom == 1)
                    return;

                e.Handled = true;

                Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(e.ManipulationOrigin);
                var curpos = this.getCurrentPosition();
                this.OSM_Map.ZoomAboutLogicalPoint(zoom, logicalPoint.X, logicalPoint.Y);

                if (this.OSM_Map.ViewportWidth > 1)
                    this.OSM_Map.ViewportWidth = 1;
            }
            // Pinch
            else
            {
                Point newPoint = lastMouseViewPort;
                newPoint.X -= e.CumulativeManipulation.Translation.X / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                newPoint.Y -= e.CumulativeManipulation.Translation.Y / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;

                //normalize in (0,1) range
                newPoint.X = Math.Min(Math.Max(newPoint.X, 0), 1);
                newPoint.Y = Math.Min(Math.Max(newPoint.Y, 0), 1);

                // (Y or X) + viewportwidth < 1
                if (newPoint.X + this.OSM_Map.ViewportWidth > 1)
                    newPoint.X = 1 - this.OSM_Map.ViewportWidth;
                if (newPoint.Y + this.OSM_Map.ViewportWidth > 1)
                    newPoint.Y = 1 - this.OSM_Map.ViewportWidth;

                this.OSM_Map.ViewportOrigin = newPoint;
            }
        }

        public void navigateToCoordinate(Point p, double zoom)
        {
            navigateToCoordinate(new System.Device.Location.GeoCoordinate(p.Y, p.X), zoom);
        }

        public void navigateToCoordinate(System.Device.Location.GeoCoordinate geoCoordinate, double zoom)
        {
            Point p = OSMHelpers.WorldToTilePos(geoCoordinate.Longitude, geoCoordinate.Latitude, zoom);
            double xRelative = (p.X - fixX) / Math.Pow(2, zoom);
            double yRelative = (p.Y - fixY) / Math.Pow(2, zoom);

            this.OSM_Map.ViewportWidth = (1.0 / Math.Pow(2, zoom));
            this.OSM_Map.ViewportOrigin = new Point(xRelative, yRelative);
            // THINK ABOUT THIS 
            zoomCount *= zoom;
        }

        public void zoomToWorldView()
        {
            // USED FOR UNZOOM 
            this.OSM_Map.ViewportOrigin = new Point(0.0, 0.0);
            this.OSM_Map.ViewportWidth = 1;
            zoomCount = 1;
        }

        public Point getCurrentPosition()
        {
            var tilecount = Math.Pow(2, this.CurrentZoom);

            var xcoord = this.OSM_Map.ViewportOrigin.X * tilecount + fixX;
            var ycoord = this.OSM_Map.ViewportOrigin.Y * tilecount + fixY;

            var res = OSMHelpers.TileToWorldPos(xcoord, ycoord, this.CurrentZoom);

            return res;
        }
    }
}

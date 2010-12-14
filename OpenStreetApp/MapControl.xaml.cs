using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Reactive;
namespace OpenStreetApp
{
    public partial class MapControl : UserControl
    {
        Point lastMouseLogicalPos = new Point();
        Point lastMouseViewPort = new Point();
        Point lastOSMPoint = new Point();
        double zoom = 1;
        double fixX = 0, fixY = 0;

        #region Source

        /// <summary>
        /// Source Dependency Property
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(TileSource), typeof(MapControl),
                new PropertyMetadata((TileSource)null,
                    new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// Gets or sets the Source property. This dependency property 
        /// indicates the currently used TileSource for the map.
        /// </summary>
        public TileSource Source
        {
            get { return (TileSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Source property.
        /// </summary>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl target = (MapControl)d;
            TileSource oldValue = (TileSource)e.OldValue;
            TileSource newValue = target.Source;
            target.OnSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Source property.
        /// </summary>
        protected virtual void OnSourceChanged(TileSource oldValue, TileSource newValue)
        {
            if (oldValue != null && newValue != null)
            {
                var oldpos = this.getCurrentPosition();
                var oldzoom = this.CurrentZoom;
                this.TileLayer.TileSources.Clear();
                this.TileLayer.TileSources.Add(newValue);
                this.navigateToCoordinate(oldpos, oldzoom);
            }
            else
            {
                this.TileLayer.TileSources.Clear();
                this.TileLayer.TileSources.Add(newValue);
            }
        }

        #endregion

        public double CurrentZoom
        {
            get
            {
                return this.OSM_Map.ZoomLevel;
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

        private void OSM_Map_OnDoubleClick()
        {
            this.OSM_Map.ZoomLevel *= 1.5;
        }

        public void navigateToCoordinate(Point p, double zoom)
        {
            navigateToCoordinate(new System.Device.Location.GeoCoordinate(p.Y, p.X), zoom);
        }

        public void navigateToCoordinate(System.Device.Location.GeoCoordinate geoCoordinate, double zoom)
        {
            this.OSM_Map.Center = geoCoordinate;
        }

        public void zoomToWorldView()
        {
            // USED FOR UNZOOM 
            this.OSM_Map.ZoomLevel = 1;
        }

        public Point getCurrentPosition()
        {
            var res = new Point();

            res.X = this.OSM_Map.TargetCenter.Longitude;
            res.Y = this.OSM_Map.TargetCenter.Latitude;

            return res;
        }
    }
}

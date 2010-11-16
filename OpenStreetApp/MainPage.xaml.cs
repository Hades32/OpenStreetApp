using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System.Device.Location;
using System.Windows.Controls.Primitives;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;
        GeoPosition<GeoCoordinate> lastKnownPosition;
        Point lastMouseLogicalPos = new Point();
        Point lastMouseViewPort = new Point();
        Point lastOSMPoint = new Point();
        double zoom = 1;
        double zoomCount = 1.0;


        public MainPage()
        {
            InitializeComponent();

            this.OSM_Map.Source = new OSMTileSource();

            // Implement double click
            Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.OSM_Map, "MouseLeftButtonUp")
            .BufferWithTimeOrCount(TimeSpan.FromSeconds(1), 2)
            .Subscribe(new Action<IList<IEvent<MouseButtonEventArgs>>>(
                eventList =>
                {
                    if (eventList.Count >= 2)
                        // subscribing directly on that dispatcher didn't work...
                        this.Dispatcher.BeginInvoke(OSM_Map_OnDoubleClick);
                }));


            this.OSM_Map.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(OSM_Map_ManipulationDelta);
            
            // Initialize GeoLocation Listener
            watcher = new GeoCoordinateWatcher();
            watcher.MovementThreshold = 20;

            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            watcher.Start();
            lastKnownPosition = watcher.Position;
        }

        //Multi-Touch working
        void OSM_Map_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // Zoom
            if (e.DeltaManipulation.Scale.X != 0 || e.DeltaManipulation.Scale.Y != 0)
            {
                // zoom by average of X and Y scaling
                var zoom = (Math.Abs(e.DeltaManipulation.Scale.X) + Math.Abs(e.DeltaManipulation.Scale.Y)) / 2.0;

                Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
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
                this.OSM_Map.ViewportOrigin = newPoint;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            lastKnownPosition = e.Position;
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch(e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // READ ARTICLE FOR POPUP - ERROR Message

                case GeoPositionStatus.Ready:
                   this.ApplicationTitle.Text += e.Status.ToString();
                   break;
                 
                default:
                   break;
            }    
        }

        private void geoLocationButton_Click(object sender, EventArgs e)
        {
            // FAKE EVENT
            watcher_PositionChanged(this, new GeoPositionChangedEventArgs<GeoCoordinate>(
                new GeoPosition<GeoCoordinate>(new DateTimeOffset(), new GeoCoordinate(48.24, 9.59))));

            Point p = OSMHelpers.WorldToTilePos(lastKnownPosition.Location.Longitude, lastKnownPosition.Location.Latitude, 12);
            double xRelative = p.X / Math.Pow(2,12);
            double yRelative = p.Y / Math.Pow(2,12);
            openButton_Click(this, null);
            // DEBUG CODE this.ApplicationTitle.Text = "X-Tile: " + (int)p.X + "Relative: " + xRelative; 
            this.OSM_Map.ZoomAboutLogicalPoint(12, xRelative, yRelative);
            zoomCount *= 12;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void openButton_Click(object sender, EventArgs e)
        {
            ContextMenuPopup.IsOpen = true;
        }

        private void showFavoritesButton_Click(object sender, EventArgs e)
        {

        }
        
        private void POIButton_Click(object sender, EventArgs e)
        {
            // USED FOR UNZOOM 
            this.OSM_Map.ViewportOrigin = new Point(0.0, 0.0);
            this.OSM_Map.ZoomAboutLogicalPoint(1.0 / zoomCount, 0, 0);
            zoomCount = 1;
        }

        private void preferencesButton_Click(object sender, EventArgs e)
        {

        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {

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
            zoomCount*=2;
            Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
            this.OSM_Map.ZoomAboutLogicalPoint(zoom / newzoom, logicalPoint.X, logicalPoint.Y);
            zoom = newzoom;
        }

        private void ContextMenuPopup_Opened(object sender, EventArgs e)
        {
            this.OSM_Map.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuPopup.IsOpen = false;
            this.OSM_Map.Visibility = System.Windows.Visibility.Visible;
            if(!String.IsNullOrEmpty(this.TargetInput.Text))
            {
                navigateToInputAdress(this.TargetInput.Text);
            }         
        }

        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuPopup.IsOpen = false;
            this.OSM_Map.Visibility = System.Windows.Visibility.Visible;
        }

        private void navigateToInputAdress(String inputAdressString)
        {
            String encoded = System.Net.HttpUtility.UrlEncode(inputAdressString);
            this.ApplicationTitle.Text = encoded;
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += (sender, e) =>
            {
                Console.WriteLine(e.Result);
            };
            Uri adress = new Uri("http://geocoding.cloudmade.com/1a8bcc813f9646519c9d2b12e92c69b2/geocoding/v2/find.js?query=" + encoded);
            Console.WriteLine(adress);
            wc.DownloadStringAsync(adress);
        }
    }
}
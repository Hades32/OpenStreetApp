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
        bool duringDrag = false;
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

            // Initialize GeoLocation Listener
            watcher = new GeoCoordinateWatcher();
            watcher.MovementThreshold = 20;

            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            watcher.Start();
            lastKnownPosition = watcher.Position;
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
            this.ApplicationTitle.Text = "X-Tile: " + (int)p.X + "Relative: " + xRelative; 
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
            lastMouseLogicalPos = e.GetPosition(this.OSM_Map);
            lastMouseViewPort = this.OSM_Map.ViewportOrigin;
            duringDrag = true;
        }

        private void OSM_Map_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            duringDrag = false;
            this.OSM_Map.UseSprings = true;
        }

        private void OSM_Map_OnDoubleClick()
        {
            var newzoom = zoom / 2.0;
            zoomCount*=2;
            Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
            this.OSM_Map.ZoomAboutLogicalPoint(zoom / newzoom, logicalPoint.X, logicalPoint.Y);
            zoom = newzoom;
        }

        private void OSM_Map_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (duringDrag)
            {
                Point newPoint = lastMouseViewPort;
                Point thisMouseLogicalPos = e.GetPosition(this.OSM_Map);
                newPoint.X += (lastMouseLogicalPos.X - thisMouseLogicalPos.X) / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                newPoint.Y += (lastMouseLogicalPos.Y - thisMouseLogicalPos.Y) / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                this.OSM_Map.ViewportOrigin = newPoint;
            }
        }
    }
}
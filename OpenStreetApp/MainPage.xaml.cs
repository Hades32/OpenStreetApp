using System;
using System.Device.Location;
using System.Windows;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;
        GeoPosition<GeoCoordinate> lastKnownPosition;


        public MainPage()
        {
            InitializeComponent();

            //this.OSM_Map.Source = new OSMTileSource();

            CloudeMadeService.authorize(() =>
                this.Dispatcher.BeginInvoke(() =>
                        this.OSM_Map.Source = new CloudeMadeTileSource()));



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
            switch (e.Status)
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

            this.OSM_Map.navigateToCoordinate(lastKnownPosition.Location, 12);
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
            this.OSM_Map.zoomToWorldView();
        }

        private void preferencesButton_Click(object sender, EventArgs e)
        {

        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {

        }

        private void ContextMenuPopup_Opened(object sender, EventArgs e)
        {
            this.OSM_Map.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuPopup.IsOpen = false;
            this.OSM_Map.Visibility = System.Windows.Visibility.Visible;
            if (!String.IsNullOrEmpty(this.TargetInput.Text))
            {
                this.OSM_Map.navigateToInputAdress(this.TargetInput.Text);
            }
        }

        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuPopup.IsOpen = false;
            this.OSM_Map.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
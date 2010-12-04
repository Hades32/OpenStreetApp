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

            //TODO
            //OSA_Configuration.Instance.load(stream);
            //show loading animation while authorizing

            this.DataContext = OSA_Configuration.Instance;

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
                    break;
                case GeoPositionStatus.Ready:
                    break;

                default:
                    break;
            }
        }

        private void geoLocationButton_Click(object sender, EventArgs e)
        {
            lastKnownPosition = this.watcher.Position;
            Point p = new Point(this.lastKnownPosition.Location.Latitude, this.lastKnownPosition.Location.Longitude);
            this.OSM_Map.navigateToCoordinate(p, 16);
            
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
            NavigationService.Navigate(new Uri("/PreferencesPage.xaml", UriKind.Relative));
        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {
            this.OSM_Map.navigateToCoordinate(new GeoCoordinate(48.399833, 9.994923), 12);
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
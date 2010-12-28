﻿using System;
using System.Device.Location;
using System.Windows;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;
        GeoPosition<GeoCoordinate> lastKnownPosition;

        // Stores the last user-searched Location.
        public static Location lastSearchedLocation = null;
        public static Point currentPosition = new Point();

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

            watcher.StatusChanged += watcher_StatusChanged;
            watcher.PositionChanged += watcher_PositionChanged;

            if ((!(System.Diagnostics.Debugger.IsAttached)) && OSA_Configuration.Instance.UseCurrentLocationSetting)
            {
                watcher.PositionChanged += watcher_initialLocationing;
            }

            watcher.Start();
            System.Diagnostics.Debug.WriteLine("watcher started");
        }

        void watcher_initialLocationing(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            System.Diagnostics.Debug.WriteLine("user wanted to start at his location");
            lastKnownPosition = e.Position;
            Point p = new Point(this.lastKnownPosition.Location.Longitude, this.lastKnownPosition.Location.Latitude);
            this.OSM_Map.navigateToCoordinate(p, 16);
            watcher.PositionChanged -= watcher_initialLocationing;
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            System.Diagnostics.Debug.WriteLine("receive coordinate, now setting");
            lastKnownPosition = e.Position;
            System.Diagnostics.Debug.WriteLine("new coordinate is now:" + lastKnownPosition.Location.Latitude + ", " + lastKnownPosition.Location.Longitude);
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // READ ARTICLE FOR POPUP - ERROR Message
                    break;
                case GeoPositionStatus.Ready:
                    System.Diagnostics.Debug.WriteLine("service is now ready");
                    break;

                default:
                    break;
            }
        }

        private void geoLocationButton_Click(object sender, EventArgs e)
        {
            lastKnownPosition = this.watcher.Position;
            Point p = new Point(this.lastKnownPosition.Location.Longitude, this.lastKnownPosition.Location.Latitude);
            this.OSM_Map.navigateToCoordinate(p, 16);
            this.OSM_Map.addPushpin(lastKnownPosition.Location);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void openButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void showFavoritesButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FavoritesPage.xaml", UriKind.Relative));
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
            System.Diagnostics.Debug.WriteLine(this.OSM_Map.getCurrentPosition());
            currentPosition = this.OSM_Map.getCurrentPosition();
            NavigationService.Navigate(new Uri("/AddFavorite.xaml", UriKind.Relative));


            // THE ULM BUTTON
            //this.OSM_Map.navigateToCoordinate(new GeoCoordinate(48.399833, 9.994923), 12);
        }

        private void ContextMenuPopup_Opened(object sender, EventArgs e)
        {
            this.OSM_Map.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (lastSearchedLocation != null)
            {
                GeoCoordinate nC = new GeoCoordinate(lastSearchedLocation.Latitude, lastSearchedLocation.Longitude);
                this.OSM_Map.navigateToCoordinate(nC, 16);
                lastSearchedLocation = null;
            }
        }
    }
}
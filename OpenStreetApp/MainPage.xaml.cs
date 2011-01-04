﻿using System;
using System.Device.Location;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Stores the last user-searched Location.
        public static GeoCoordinate targetLocation = null;
        public static LocationCollection newRoute = null;

        public static Point currentPosition = new Point();

        public MainPage()
        {
            InitializeComponent();

            //TODO
            //OSA_Configuration.Instance.load(stream);
            //show loading animation while authorizing

            this.DataContext = OSA_Configuration.Instance;

            if ((!(System.Diagnostics.Debugger.IsAttached)) && OSA_Configuration.Instance.UseCurrentLocationSetting)
            {
                App.watcher.PositionChanged += watcher_initialLocationing;
            }
        }

        void watcher_initialLocationing(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            System.Diagnostics.Debug.WriteLine("user wanted to start at his location");
            App.lastKnownPosition = e.Position;
            Point p = new Point(App.lastKnownPosition.Location.Longitude, App.lastKnownPosition.Location.Latitude);
            this.OSM_Map.navigateToCoordinate(p, 16);
            App.watcher.PositionChanged -= watcher_initialLocationing;
        }

        private void geoLocationButton_Click(object sender, EventArgs e)
        {
            Point p = new Point(App.lastKnownPosition.Location.Longitude, App.lastKnownPosition.Location.Latitude);
            this.OSM_Map.addPushpin(App.lastKnownPosition.Location);
            this.OSM_Map.navigateToCoordinate(p, 16);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            CloudeMadeService.getRoute(new GeoCoordinate(48.458756, 10.100310),
                                                            new GeoCoordinate(48.426353, 10.115564),
                                                            null, (wps) =>
                {
                    var waypoints = new LocationCollection();
                    foreach (var item in wps)
                    {
                        waypoints.Add(item.Coordinate);
                    }
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            this.OSM_Map.setRoute(waypoints);
                            this.OSM_Map.navigateToCoordinate(waypoints[waypoints.Count / 2], 8);
                        });
                });
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

        private void routeBtn_Click(object sender, EventArgs e)
        {

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
            if (targetLocation != null)
            {
                this.OSM_Map.navigateToCoordinate(targetLocation, 16);
                targetLocation = null;
            }
            if (newRoute != null)
            {
                this.OSM_Map.setRoute(newRoute);
                newRoute = null;
            }
        }
    }
}
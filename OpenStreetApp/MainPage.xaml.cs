using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static Point currentPosition = new Point();

        public MainPage()
        {
            InitializeComponent();

            //TODO
            //OSA_Configuration.Instance.load(stream);
            //show loading animation while authorizing

            this.DataContext = OSA_Configuration.Instance;
        }

        private void geoLocationButton_Click(object sender, EventArgs e)
        {
            if (App.lastKnownPosition != null)
            {
                Point p = new Point(App.lastKnownPosition.Location.Longitude, App.lastKnownPosition.Location.Latitude);
                this.OSM_Map.addPushpin(App.lastKnownPosition.Location);
                this.OSM_Map.navigateToCoordinate(p, 16);
            }
            else
            {
                MessageBox.Show("Sorry, no GPS available");
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            App.My.navigateWithResult("/SearchPage.xaml", "search", null);
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
            App.My.navigateWithResult("/RoutePage.xaml", "route", null);
        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {
            MainPage.currentPosition = this.OSM_Map.getCurrentPosition();
            App.My.navigateWithResult("/FavoritesPage.xaml", "favorites", null);
        }

        private void ContextMenuPopup_Opened(object sender, EventArgs e)
        {
            this.OSM_Map.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //came from SearchPage?
            var searchresult = App.My.getNavigationResult("/SearchPage.xaml");
            if (!string.IsNullOrEmpty(searchresult.Key))
            {
                var targetLocation = (Location)searchresult.Value;
                if (targetLocation != null)
                {
                    var coords = new GeoCoordinate(targetLocation.Latitude, targetLocation.Longitude);
                    this.OSM_Map.navigateToCoordinate(coords, 16);
                }
            }
            //came from FavoritesPage?
            var favresult = App.My.getNavigationResult("/FavoritesPage.xaml");
            if (!string.IsNullOrEmpty(favresult.Key))
            {
                var targetLocation = (Location)favresult.Value;
                if (targetLocation != null)
                {
                    var coords = new GeoCoordinate(targetLocation.Latitude, targetLocation.Longitude);
                    this.OSM_Map.navigateToCoordinate(coords, 16);
                }
            }

            //came from RoutePage?
            var routeresult = App.My.getNavigationResult("/RoutePage.xaml");
            if (!string.IsNullOrEmpty(routeresult.Key))
            {
                var wps = (IEnumerable<Waypoint>)routeresult.Value;
                if (wps != null)
                {
                    var waypoints = new LocationCollection();
                    foreach (var item in wps)
                    {
                        waypoints.Add(item.Coordinate);
                    }
                    this.OSM_Map.setRoute(waypoints);
                }
            }
            /*if (newRoute != null)
            {
                this.OSM_Map.setRoute(newRoute);
                this.OSM_Map.navigateToCoordinate(newRoute[newRoute.Count / 2], 10);
                newRoute = null;
            }*/
        }

        private void clearMap_Click(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class RoutePage : PhoneApplicationPage
    {
        public RoutePage()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationResults.setOrAdd(typeof(SearchPage), new KeyValuePair<string, object>("start", null));
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void targetBtn_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationResults.setOrAdd(typeof(SearchPage), new KeyValuePair<string, object>("target", null));
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void routeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.routeBtn.IsEnabled = false;
            this.startBtn.IsEnabled = false;
            this.targetBtn.IsEnabled = false;
            this.progress.Visibility = System.Windows.Visibility.Visible;
            this.progress.IsIndeterminate = true;

            var start_loc = (Location)this.State["start"];
            var end_loc = (Location)this.State["target"];
            var start = new GeoCoordinate(start_loc.Latitude, start_loc.Longitude);
            var end = new GeoCoordinate(end_loc.Latitude, end_loc.Longitude);
            CloudeMadeService.getRoute(start,
                                        end,
                                        null, (wps) =>
                                        {
                                            var res = App.NavigationResults[this.GetType()];
                                            App.NavigationResults[this.GetType()] =
                                                new KeyValuePair<string, object>(res.Key, wps);
                                            this.Dispatcher.BeginInvoke(() =>
                                                NavigationService.GoBack());
                                        });
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.NavigationResults.ContainsKey(typeof(SearchPage)))
            {
                var searchresult = App.NavigationResults.getOrDefault(typeof(SearchPage));
                this.State[searchresult.Key] = searchresult.Value;
                App.NavigationResults.Remove(typeof(SearchPage));
            }

            if (this.State.ContainsKey("start") && this.State.ContainsKey("target"))
            {
                this.routeBtn.IsEnabled = true;
            }
        }
    }
}
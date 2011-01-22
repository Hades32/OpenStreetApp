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
            App.My.navigateWithResult("/SearchPage.xaml", "start");
        }

        private void targetBtn_Click(object sender, RoutedEventArgs e)
        {
            App.My.navigateWithResult("/SearchPage.xaml", "target");
        }

        private void routeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.routeBtn.IsEnabled = false;
            this.startBtn.IsEnabled = false;
            this.targetBtn.IsEnabled = false;
            this.progress.Visibility = System.Windows.Visibility.Visible;
            this.progress.IsIndeterminate = true;

            GeoCoordinate start;
            if (this.State.getOrDefault("currentposition") as bool? == true)
            {
                start = App.lastKnownPosition.Location;
            }
            else
            {
                var start_loc = (Location)this.State["start"];
                start = new GeoCoordinate(start_loc.Latitude, start_loc.Longitude);
            }

            var end_loc = (Location)this.State["target"];
            var end = new GeoCoordinate(end_loc.Latitude, end_loc.Longitude);

            CloudeMadeService.getRoute(start,
                                        end,
                                        null, (wps) =>
                                        {
                                            App.My.putNavigationResult("/RoutePage.xaml", wps);
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

            var searchresult = App.My.getNavigationResult("/SearchPage.xaml");
            if (string.IsNullOrEmpty(searchresult.Key) == false)
            {
                this.State[searchresult.Key] = searchresult.Value;
            }
            updateGuiState();
        }

        private void updateGuiState()
        {
            this.routeBtn.IsEnabled = (this.State.ContainsKey("start")
                                    || this.State.getOrDefault("currentposition") as bool? == true)
                                    && this.State.ContainsKey("target");

            if (this.State.ContainsKey("start"))
                this.startBtnTB.Text = ((Location)this.State["start"]).ToShortString();

            if (this.State.getOrDefault("currentposition") as bool? == true)
            {
                this.startBtnTB.Text = "Aktuelle Position";
                this.startBtn.IsEnabled = false;
            }
            else
            {
                this.startBtnTB.Text = "Start auswählen";
                this.startBtn.IsEnabled = true;
            }

            if (this.State.ContainsKey("target"))
                this.targetBtnTB.Text = ((Location)this.State["target"]).ToShortString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.State.setOrAdd("currentposition", true);
            updateGuiState();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.State.setOrAdd("currentposition", false);
            updateGuiState();
        }
    }
}
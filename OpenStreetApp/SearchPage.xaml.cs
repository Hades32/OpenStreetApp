using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public List<Location> Locations  { private set; get; }

        public SearchPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.TargetInput.Text))
            {
                //System.Diagnostics.Debug.WriteLine(Locations);
                Locations = OSMHelpers.InputAdressToLocations(this.TargetInput.Text);
                System.Diagnostics.Debug.WriteLine(Locations);
            }
        }

        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}
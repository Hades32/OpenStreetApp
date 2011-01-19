using System.Windows;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace OpenStreetApp
{
    public partial class AddFavorite : PhoneApplicationPage
    {
        private Location current = null;

        public AddFavorite()
        {
            OSMHelpers.GeoPositionToLocation(MainPage.currentPosition, onLocationReceived);
            InitializeComponent();
        }

        private void onLocationReceived(Location current)
        {
            this.LocationInfo.Text = current.ToString();
            this.current = current;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            var res = App.NavigationResults[this.GetType()];
            App.NavigationResults[this.GetType()] =
                new KeyValuePair<string, object>(res.Key, this.current);
            NavigationService.GoBack();
        }
    }
}
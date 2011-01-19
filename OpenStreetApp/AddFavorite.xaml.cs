using System.Windows;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

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
            OSA_Configuration.Instance.FavoritesSetting.Add(this.current);
            OSA_Configuration.Instance.Save();
            NavigationService.GoBack();
        }
    }
}
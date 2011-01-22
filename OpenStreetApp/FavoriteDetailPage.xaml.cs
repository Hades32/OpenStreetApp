using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class FavoriteDetailPage : PhoneApplicationPage
    {
        private Location current = null;

        public FavoriteDetailPage()
        {
            InitializeComponent();
            var searchresult = App.My.getNavigationResult("/FavoriteDetailPage.xaml");
            current = (Location)searchresult.Value;

            this.LocationInfo.Text = "" + current.LocationListView;
            this.latitude.Text = "Latitude: " + current.Latitude;
            this.longitude.Text = "Longitude: " + current.Longitude;
            this.description.Text = "" + current.Description;
        }
    }
}
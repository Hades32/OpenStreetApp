using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class FavoriteDetailPage : PhoneApplicationPage
    {
        private Location current = null;

        public FavoriteDetailPage()
        {
            InitializeComponent();
            var res = App.My.getNavigationResult("/FavoriteDetailPage.xaml");
            if (!string.IsNullOrEmpty(res.Key))
            {
                this.current = (Location)res.Value;
                this.LocationInfo.Text = "" + current.ToString();
                this.latitude.Text = "Latitude: " + current.Latitude;
                this.longitude.Text = "Longitude: " + current.Longitude;
                this.description.Text = "" + current.Description;
            }       
        }
    }
}
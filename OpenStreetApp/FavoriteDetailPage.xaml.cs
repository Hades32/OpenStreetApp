using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class FavoriteDetailPage : PhoneApplicationPage
    {
        private Location current = null;

        public FavoriteDetailPage()
        {
            InitializeComponent();
            var res = App.My.getNavigationParameter();
            if (res != null)
            {
                this.current = (Location)res;
                this.LocationInfo.Text = "" + current.ToString();
                this.latitude.Text = "Latitude: " + current.Latitude;
                this.longitude.Text = "Longitude: " + current.Longitude;
                this.description.Text = "" + current.Description;
            }       
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            current.Description = this.description.Text;
            NavigationService.GoBack();
        }
    }
}
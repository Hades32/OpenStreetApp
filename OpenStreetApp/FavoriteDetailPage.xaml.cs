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
    public partial class FavoriteDetailPage : PhoneApplicationPage
    {
        private Location current = null;

        public FavoriteDetailPage()
        {
            InitializeComponent();
            var searchresult = App.NavigationResults.getOrDefault(typeof(FavoriteDetailPage));
            current = (Location)searchresult.Value;

            this.LocationInfo.Text = "" + current.ToString();
            this.latitude.Text = "Latitude: " + current.Latitude;
            this.longitude.Text = "Longitude: " + current.Longitude;
            this.description.Text = "" + current.Description;
        }
    }
}
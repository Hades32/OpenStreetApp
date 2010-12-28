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
using System.IO.IsolatedStorage;
using System.IO;

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
            OSA_Configuration.Instance.addFavorite(this.current);   
        }
    }
}
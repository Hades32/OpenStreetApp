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
    public partial class FavoritesPage : PhoneApplicationPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
            this.DataContext = OSA_Configuration.Instance;
        }

        private void detail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            Location selected = (sender as MenuItem).DataContext as Location;
            OSA_Configuration.Instance.FavoritesSetting.Remove(selected);
        }

        private void favorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var res = App.NavigationResults[this.GetType()];
            App.NavigationResults[this.GetType()] =
                new KeyValuePair<string, object>(res.Key, this.favorites.SelectedItem);
            NavigationService.GoBack();
        }

        private void addFavorite_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddFavorite.xaml", UriKind.Relative));
        }
    }
}
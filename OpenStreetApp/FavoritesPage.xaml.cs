using System.Windows;
using System.Windows.Controls;
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
            Location selected = (sender as MenuItem).DataContext as Location;
            App.My.navigateWithResult("/FavoriteDetailPage.xaml", "favoriteDetail");
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            Location selected = (sender as MenuItem).DataContext as Location;
            OSA_Configuration.Instance.FavoritesSetting.Remove(selected);
            OSA_Configuration.Instance.FavoritesSetting = OSA_Configuration.Instance.FavoritesSetting;
        }

        private void favorites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.My.putNavigationResult("/FavoritesPage.xaml", this.favorites.SelectedItem);
            NavigationService.GoBack();
        }

        private void addFavorite_Click(object sender, RoutedEventArgs e)
        {
            App.My.navigateWithResult("/AddFavorite.xaml", "addFavorite");
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var res = App.My.getNavigationResult("/AddFavorite.xaml");
            if (!string.IsNullOrEmpty(res.Key))
            {
                var targetLocation = (Location)res.Value;
                if (targetLocation != null)
                {
                    OSA_Configuration.Instance.FavoritesSetting.Add(targetLocation);
                    OSA_Configuration.Instance.FavoritesSetting = OSA_Configuration.Instance.FavoritesSetting;
                }
            }
        }
    }
}
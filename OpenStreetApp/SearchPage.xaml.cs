using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class SearchPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string prop)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(prop));
        }

        #region Locations

        /// <summary>
        /// Locations Property
        /// </summary>
        private List<Location> _Locations;

        /// <summary>
        /// Gets or sets the Locations property. This property indicates
        /// the location results to the user query.
        /// </summary>
        public List<Location> Locations
        {
            get { return this._Locations; }
            set
            {
                if (value != this._Locations)
                {
                    this._Locations = value;
                    this.RaisePropertyChanged("Locations");
                }
            }
        }

        #endregion

        #region LastSearchedLocations

        /// <summary>
        /// Locations Property
        /// </summary>
        private ObservableCollection<String> _LastSearchedLocations;

        /// <summary>
        /// Gets or sets the Locations property. This property indicates
        /// the location results to the user query.
        /// </summary>
        public ObservableCollection<String> LastSearchedLocations
        {
            get { return this._LastSearchedLocations; }
            set
            {
                if (value != this._LastSearchedLocations)
                {
                    this._LastSearchedLocations = value;
                    this.RaisePropertyChanged("LastSearchedLocations");
                }
            }
        }

        #endregion

        public SearchPage()
        {
            InitializeComponent();
            this.DataContext = this;
            this.LastSearchedLocations = OSA_Configuration.Instance.LastSearchedLocationsSetting;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.TargetInput.Text.Trim()))
            {
                this.Locations = null;
                this.results.IsEnabled = true;
                this.setIsLoading(true);
                //adding searched location manually, due to not beeing able to bind this properly. 
                //observable collection will however do the syncronization for the ui.
                //the code checks wether the last searched locations have reached 10, therefor deleting the first (FIFO)
                if (!(sender == this.lastSearched) && !OSA_Configuration.Instance.LastSearchedLocationsSetting.Contains(this.TargetInput.Text))
                {
                    ObservableCollection<String> newSearchedLocations = OSA_Configuration.Instance.LastSearchedLocationsSetting;
                    if (newSearchedLocations.Count == 10)
                    {
                        newSearchedLocations.RemoveAt(9);
                    }
                    newSearchedLocations.Insert(0, this.TargetInput.Text.Trim());
                    OSA_Configuration.Instance.LastSearchedLocationsSetting = newSearchedLocations;
                }
                OSMHelpers.InputAdressToLocations(this.TargetInput.Text.Trim(), new Action<List<Location>>(onLocationsReceived));
                this.SearchPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.ResultPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void onLocationsReceived(List<Location> newLocations)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.setIsLoading(false);
                if (newLocations.Count > 0)
                {
                    newLocations.Sort();
                    this.Locations = newLocations;
                }
                else
                {
                    this.results.IsEnabled = false;
                    this.Locations = new List<Location>() { new Location() { Description = "No Results Found" } };
                }
            });
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.My.putNavigationResult("/SearchPage.xaml", this.results.SelectedItem);
            NavigationService.GoBack();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (this.ResultPanel.Visibility == System.Windows.Visibility.Visible)
            {
                e.Cancel = true;
                this.ResultPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.SearchPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void TargetInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonOK_Click(this, null);
            }
        }

        private void detail_Click(object sender, RoutedEventArgs e)
        {
            Location selected = (sender as MenuItem).DataContext as Location;
            App.My.navigateWithResult("/FavoriteDetailPage.xaml", "favoriteDetail", selected);
        }

        private void lastSearched_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TargetInput.Text = (String)this.lastSearched.SelectedItem;
            buttonOK_Click(this.lastSearched, null);
        }

        protected void setIsLoading(bool loading)
        {
            this.progress.IsIndeterminate = loading;
            this.progress.Visibility = loading ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            buttonOK_Click(this, null);
        }
    }
}
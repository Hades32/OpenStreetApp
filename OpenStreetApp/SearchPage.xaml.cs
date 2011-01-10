using System;
using System.Collections.Generic;
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

        #region result handling

        private static Location result = null;

        /// <summary>
        /// This methods returns the last selected location and then
        /// DELETES the result. 
        /// </summary>
        /// <returns>The last selected location</returns>
        public static Location popResult()
        {
            var res = SearchPage.result;
            SearchPage.result = null;
            return res;
        }

        #endregion

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

        public SearchPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.TargetInput.Text))
            {
                OSMHelpers.InputAdressToLocations(this.TargetInput.Text, new Action<List<Location>>(onLocationsReceived));
                this.SearchPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.ResultPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void onLocationsReceived(List<Location> newLocations)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.Locations = newLocations;
            });
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchPage.result = (Location)this.results.SelectedItem;
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
    }
}
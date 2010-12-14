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
using System.ComponentModel;

namespace OpenStreetApp
{
    public partial class SearchPage : PhoneApplicationPage, INotifyPropertyChanged
    {
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
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Locations"));
                }
            }
        }

        #endregion
      

        public SearchPage()
        {
            InitializeComponent();
            this.DataContext = this;
            this.PropertyChanged += (a, b) => { };
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainPage.lastSearchedLocation = (Location) this.results.SelectedItem;
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
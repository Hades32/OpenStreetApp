using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;

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
            if (!String.IsNullOrEmpty(this.TargetInput.Text))
            {
                //adding searched location manually, due to not beeing able to bind this properly. 
                //observable collection will however do the syncronization for the ui.
                //the code checks wether the last searched locations have reached 10, therefor deleting the first (FIFO)
                if(!(sender == this.lastSearched))
                {
                    ObservableCollection<String> newSearchedLocations = OSA_Configuration.Instance.LastSearchedLocationsSetting;
                    if (newSearchedLocations.Count == 10)
                    {
                        newSearchedLocations.RemoveAt(0);
                    }
                    newSearchedLocations.Insert(0, this.TargetInput.Text);
                    OSA_Configuration.Instance.LastSearchedLocationsSetting = newSearchedLocations;
                }
                OSMHelpers.InputAdressToLocations(this.TargetInput.Text, new Action<List<Location>>(onLocationsReceived));
                this.SearchPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.ResultPanel.Visibility = System.Windows.Visibility.Visible;             
            }
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
            var res = App.NavigationResults[this.GetType()];
            App.NavigationResults[this.GetType()] =
                new KeyValuePair<string, object>(res.Key, this.results.SelectedItem);
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

        }

        private void lastSearched_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TargetInput.Text = (String)this.lastSearched.SelectedItem;
            buttonOK_Click(this.lastSearched, null);
        }
    }
}
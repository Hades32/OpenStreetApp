using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Threading;

namespace OpenStreetApp
{
    public class OSA_Configuration : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region it's a Singleton!
        private static OSA_Configuration instance;
        public static OSA_Configuration Instance
        {
            get
            {
                if (instance == null)
                    instance = new OSA_Configuration();
                return instance;
            }
        }
        #endregion

        #region TileSource

        /// <summary>
        /// TileSource Property
        /// </summary>
        private Microsoft.Phone.Controls.Maps.TileSource _TileSource;

        /// <summary>
        /// Gets or sets the TileSource property. This property 
        /// indicates the current TileSource.
        /// </summary>
        public Microsoft.Phone.Controls.Maps.TileSource TileSource
        {
            get { return this._TileSource; }
            set
            {
                if (value != this._TileSource)
                {
                    this._TileSource = value;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TileSource"));
                }
            }
        }

        #endregion

        public IEnumerable<Microsoft.Phone.Controls.Maps.TileSource> AvailableTileSources { private set; get; }

        // Provides a Dictionary(Of TKey, TValue) that stores key-value pairs in isolated storage. 
        private IsolatedStorageSettings isolatedStore;

        #region key names for our storage
        private const string selectedTileSourceKeyName = "defaultTileSource";
        private const string useCurrentLocationKeyName = "useCurrentLocation";
        #endregion

        #region default values for our keys
        private int selectedTileSourceDefault = 1;
        private bool useCurrentLocationDefault = true;
        #endregion

        private OSA_Configuration()
        {
            try
            {
                // Get the settings for this application.
                isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }

            //avoid NULL checks
            this.PropertyChanged += (s, e) => { };
            // set some sensible defaults
            var defaultTS =
            this.AvailableTileSources = new List<Microsoft.Phone.Controls.Maps.TileSource>() 
            {
                new OSMTileSource(), new VEArialTileSource(), new VERoadTileSource(), new CloudeMadeTileSource()
            };
            this.TileSource = AvailableTileSources.ElementAt(SelectedTileSourceSetting);
        }

        public void initialize(Action callback, Dispatcher dispatcher)
        {
            CloudeMadeService.authorize(() => dispatcher.BeginInvoke(() => callback()));
        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            try
            {
                // if new value is different, set the new value.
                if (isolatedStore[Key] != value)
                {
                    isolatedStore[Key] = value;
                    valueChanged = true;
                }
            }
            catch (KeyNotFoundException)
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }
            catch (ArgumentException)
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }
            return valueChanged;
        }

        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            try
            {
                value = (valueType)isolatedStore[Key];
            }
            catch (KeyNotFoundException)
            {
                value = defaultValue;
            }
            catch (ArgumentException)
            {
                value = defaultValue;
            }

            return value;
        }


        public void Save()
        {
            isolatedStore.Save();
        }

        public bool UseCurrentLocationSetting
        {
            get
            {
                return GetValueOrDefault<bool>(useCurrentLocationKeyName, useCurrentLocationDefault);
            }
            set
            {
                AddOrUpdateValue(useCurrentLocationKeyName, value);
                Save();
            }
        }

        public int SelectedTileSourceSetting
        {
            get
            {
                return GetValueOrDefault<int>(selectedTileSourceKeyName, selectedTileSourceDefault);
            }
            set
            {
                AddOrUpdateValue(selectedTileSourceKeyName, value);
                TileSource = AvailableTileSources.ElementAt(value);
                Save();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
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
        private MultiScaleTileSource _TileSource;

        /// <summary>
        /// Gets or sets the TileSource property. This property 
        /// indicates the current TileSource.
        /// </summary>
        public MultiScaleTileSource TileSource
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

        public IEnumerable<MultiScaleTileSource> AvailableTileSources { private set; get; }

        private OSA_Configuration()
        {
            //avoid NULL checks
            this.PropertyChanged += (s, e) => { };
            // set some sensible defaults
            var defaultTS = new CloudeMadeTileSource();
            this.AvailableTileSources = new List<MultiScaleTileSource>() 
            {
                new OSMTileSource(), new VEArialTileSource(), new VERoadTileSource(), defaultTS
            };
            this.TileSource = defaultTS;
        }

        public void initialize(Action callback, Dispatcher dispatcher)
        {
            CloudeMadeService.authorize(() => dispatcher.BeginInvoke(() => callback()));
        }

        public void save(Stream stream)
        {
            //TODO save
        }

        public void load(Stream stream)
        {
            //TODO load
        }
    }
}

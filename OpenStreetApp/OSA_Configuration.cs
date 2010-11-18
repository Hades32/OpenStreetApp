using System.ComponentModel;
using System.IO;
using System.Windows.Media;

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

        private OSA_Configuration()
        {
            //avoid NULL checks
            this.PropertyChanged += (s, e) => { };
            // set some sensible defaults
            this.TileSource = new OSMTileSource();

            //this.OSM_Map.Source = new OSMTileSource();

            /*CloudeMadeService.authorize(() =>
                this.Dispatcher.BeginInvoke(() =>
                        this.OSM_Map.Source = new VEArialTileSource()//new CloudeMadeTileSource()
                        ));*/
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
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TileSource"));
                this._TileSource = value;
            }
        }

        #endregion

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

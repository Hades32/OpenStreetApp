using System.Windows;
using System.Windows.Controls;

namespace OpenStreetApp
{
    public partial class OSMControl : UserControl
    {
        #region MapCenter

        /// <summary>
        /// MapCenter Dependency Property
        /// </summary>
        public static readonly DependencyProperty MapCenterProperty =
            DependencyProperty.Register("MapCenter", typeof(Point), typeof(OSMControl),
                new PropertyMetadata((Point)new Point(48.6, 10.1),
                    new PropertyChangedCallback(OnMapCenterChanged)));

        /// <summary>
        /// Gets or sets the MapCenter property. This dependency property 
        /// indicates the coordinates of the map center.
        /// </summary>
        public Point MapCenter
        {
            get { return (Point)GetValue(MapCenterProperty); }
            set { SetValue(MapCenterProperty, value); }
        }

        /// <summary>
        /// Handles changes to the MapCenter property.
        /// </summary>
        private static void OnMapCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OSMControl target = (OSMControl)d;
            Point oldValue = (Point)e.OldValue;
            Point newValue = target.MapCenter;
            target.OnMapCenterChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the MapCenter property.
        /// </summary>
        protected virtual void OnMapCenterChanged(Point oldValue, Point newValue)
        {
            this.OnMapViewChanged();
        }

        #endregion

        #region ZoomLevel

        /// <summary>
        /// ZoomLevel Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(OSMControl),
                new PropertyMetadata((double)14.0,
                    new PropertyChangedCallback(OnZoomLevelChanged)));

        /// <summary>
        /// Gets or sets the ZoomLevel property. This dependency property 
        /// indicates the zoom level. For non-integer values the pictures are scaled appropiately.
        /// </summary>
        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ZoomLevel property.
        /// </summary>
        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OSMControl target = (OSMControl)d;
            double oldValue = (double)e.OldValue;
            double newValue = target.ZoomLevel;
            target.OnZoomLevelChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ZoomLevel property.
        /// </summary>
        protected virtual void OnZoomLevelChanged(double oldValue, double newValue)
        {
            this.OnMapViewChanged();
        }

        #endregion

        TileDownloadManager tileDownloader;

        //TODO: other data structure?
        GeoImage[] imgs = new GeoImage[40];

        public OSMControl()
        {
            InitializeComponent();

            this.tileDownloader = new TileDownloadManager();

            //add Images
            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i] = new GeoImage(this.tileDownloader);
                this.LayoutRoot.Children.Add(imgs[i].Image);
            }
        }

        protected virtual void OnMapViewChanged()
        {

        }
    }
}

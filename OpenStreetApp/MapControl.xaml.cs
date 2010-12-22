using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Reactive;
namespace OpenStreetApp
{
    public partial class MapControl : UserControl
    {
        #region Source

        /// <summary>
        /// Source Dependency Property
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(TileSource), typeof(MapControl),
                new PropertyMetadata((TileSource)null,
                    new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// Gets or sets the Source property. This dependency property 
        /// indicates the currently used TileSource for the map.
        /// </summary>
        public TileSource Source
        {
            get { return (TileSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Source property.
        /// </summary>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl target = (MapControl)d;
            TileSource oldValue = (TileSource)e.OldValue;
            TileSource newValue = target.Source;
            target.OnSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Source property.
        /// </summary>
        protected virtual void OnSourceChanged(TileSource oldValue, TileSource newValue)
        {
            if (oldValue != null && newValue != null)
            {
                var oldpos = this.getCurrentPosition();
                var oldzoom = this.CurrentZoom;
                this.TileLayer.TileSources.Clear();
                this.TileLayer.TileSources.Add(newValue);
                this.navigateToCoordinate(oldpos, oldzoom);
            }
            else
            {
                this.TileLayer.TileSources.Clear();
                this.TileLayer.TileSources.Add(newValue);
            }
        }

        #endregion

        public double CurrentZoom
        {
            get
            {
                return this.OSM_Map.ZoomLevel;
            }
        }

        public MapControl()
        {
            InitializeComponent();

            // Implement double click
            Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.touchBorder, "MouseLeftButtonUp")
            .BufferWithTimeOrCount(TimeSpan.FromSeconds(0.5), 2)
            .Subscribe(new Action<IList<IEvent<MouseButtonEventArgs>>>(
                eventList =>
                {
                    if (eventList.Count >= 2)
                        // subscribing directly on that dispatcher didn't work...
                        this.Dispatcher.BeginInvoke(OSM_Map_OnDoubleClick);
                }));
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            this.OSM_Map.CredentialsProvider = new ApplicationIdCredentialsProvider("Akc2a6v34Acf-tYc8miIU8NgDDffnkpD7TZdV69jwWk-3pt21_RCIUfba7_G5-Vl");
        }

        private void OSM_Map_OnDoubleClick()
        {
            this.OSM_Map.ZoomLevel += 0.5;
        }

        public void navigateToCoordinate(Point p, double zoom)
        {
            navigateToCoordinate(new System.Device.Location.GeoCoordinate(p.Y, p.X), zoom);
        }

        public void navigateToCoordinate(System.Device.Location.GeoCoordinate geoCoordinate, double zoom)
        {
            this.OSM_Map.Center = geoCoordinate;
            this.OSM_Map.ZoomLevel = zoom;
        }

        public void zoomToWorldView()
        {
            // USED FOR UNZOOM 
            this.OSM_Map.ZoomLevel = 1;
        }

        public Point getCurrentPosition()
        {
            var res = new Point();

            res.X = this.OSM_Map.TargetCenter.Longitude;
            res.Y = this.OSM_Map.TargetCenter.Latitude;

            return res;
        }

        private void touchBorder_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            e.Handled = true;
        }

        private void touchBorder_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            // is zoom
            if (e.DeltaManipulation.Scale.X != 0 ||
                e.DeltaManipulation.Scale.Y != 0)
            {
                e.Complete();
                var zoom = this.OSM_Map.ZoomLevel * Math.Min(e.DeltaManipulation.Scale.X,
                                                         e.DeltaManipulation.Scale.Y);
                if (zoom == this.OSM_Map.ZoomLevel)
                    zoom = this.OSM_Map.ZoomLevel * Math.Max(e.DeltaManipulation.Scale.X,
                                                         e.DeltaManipulation.Scale.Y);
                if (zoom == this.OSM_Map.ZoomLevel)
                    return;

                this.OSM_Map.ZoomLevel = zoom;
                this.OSM_Map.Center = this.OSM_Map.ViewportPointToLocation(e.ManipulationOrigin);
            }
            else // no zoom
            {
                var newpos = this.OSM_Map.LocationToViewportPoint(this.OSM_Map.Center);
                var widthConst = 1;
                var heightConst = 1;
                var dx = e.DeltaManipulation.Translation.X * widthConst;
                var dy = e.DeltaManipulation.Translation.Y * heightConst;
                newpos.X -= dx;
                newpos.Y -= dy;
                this.OSM_Map.AnimationLevel = AnimationLevel.None;
                this.OSM_Map.Center = this.OSM_Map.ViewportPointToLocation(newpos);
                this.OSM_Map.AnimationLevel = AnimationLevel.Full;
            }
            e.Handled = true;
        }

        private void touchBorder_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            e.Handled = true;
        }
    }
}

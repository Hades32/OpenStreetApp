using System;
using System.Collections.Generic;
using System.Device.Location;
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

        private MapLayer PushpinLayer = new MapLayer();
        private LocationCollection fullRoute = null;
        //prevent cross-thread problems
        private double lastKnownZoom = 1.0;

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

            Microsoft.Phone.Reactive.Observable.FromEvent<ManipulationCompletedEventArgs>(this.touchBorder, "ManipulationCompleted")
                .BufferWithTime(TimeSpan.FromSeconds(1))
                .Subscribe(new Action<IList<IEvent<ManipulationCompletedEventArgs>>>(
                eventList =>
                {
                    if (this.fullRoute != null)
                    {
                        var simpleroute = RouteSimplifier.simplifyRoute(this.fullRoute, 
                                                                        this.getMaxDistForZoomLevel(),
                                                                        this.OSM_Map.TargetBoundingRectangle);
                        this.Dispatcher.BeginInvoke(() =>
                            this.Route.Locations = simpleroute);
                    }
                }));

            this.OSM_Map.Children.Add(PushpinLayer);
        }

        private void OSM_Map_MapZoom(object sender, MapZoomEventArgs e)
        {
            this.lastKnownZoom = this.OSM_Map.TargetZoomLevel;
        }

        void Control_Loaded(object sender, RoutedEventArgs e)
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

        public void addPushpin(GeoCoordinate geoCoordinate)
        {
            Pushpin pushpin = new Pushpin();
            this.PushpinLayer.AddChild(pushpin, geoCoordinate);
        }

        public void setRoute(LocationCollection points)
        {
            this.fullRoute = points;
            this.Route.Locations = RouteSimplifier.simplifyRoute(points, getMaxDistForZoomLevel(),
                                                                 this.OSM_Map.TargetBoundingRectangle);
        }

        private double getMaxDistForZoomLevel()
        {
            if (this.lastKnownZoom > 15)
                return 0.003;
            else if (this.lastKnownZoom > 14)
                return 0.03;
            else if (this.lastKnownZoom > 13)
                return 0.3;
            else if (this.lastKnownZoom > 12)
                return 3;
            else if (this.lastKnownZoom > 11)
                return 10;
            else if (this.lastKnownZoom > 10)
                return 20;
            else if (this.lastKnownZoom > 9)
                return 30;
            else if (this.lastKnownZoom > 8)
                return 50;
            else if (this.lastKnownZoom > 7)
                return 60;
            else if (this.lastKnownZoom > 6)
                return 70;
            else if (this.lastKnownZoom > 5)
                return 80;
            else if (this.lastKnownZoom > 4)
                return 90;
            else if (this.lastKnownZoom > 3)
                return 100;
            else if (this.lastKnownZoom > 2)
                return 100;
            else
                return 100;
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
                System.Diagnostics.Debug.WriteLine("still getting zoom event");
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
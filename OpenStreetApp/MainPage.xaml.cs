using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        Point lastMouseLogicalPos = new Point();
        Point lastMouseViewPort = new Point();
        bool duringDrag = false;
        double zoom = 1;

        public MainPage()
        {
            InitializeComponent();

            this.OSM_Map.Source = new OSMTileSource();

            // Implement double click
            Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.OSM_Map, "MouseLeftButtonUp")
            .BufferWithTimeOrCount(TimeSpan.FromSeconds(1), 2)
            .Subscribe(new Action<IList<IEvent<MouseButtonEventArgs>>>(
                eventList =>
                {
                    if (eventList.Count >= 2)
                        // subscribing directly on that dispatcher didn't work...
                        this.Dispatcher.BeginInvoke(OSM_Map_OnDoubleClick);
                }));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void openButton_Click(object sender, EventArgs e)
        {

        }

        private void showFavoritesButton_Click(object sender, EventArgs e)
        {

        }

        private void preferencesButton_Click(object sender, EventArgs e)
        {

        }

        private void favoriteButton_Click(object sender, EventArgs e)
        {

        }

        private void POIButton_Click(object sender, EventArgs e)
        {

        }

        private void OSM_Map_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lastMouseLogicalPos = e.GetPosition(this.OSM_Map);
            lastMouseViewPort = this.OSM_Map.ViewportOrigin;
            duringDrag = true;
        }

        private void OSM_Map_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            duringDrag = false;
            this.OSM_Map.UseSprings = true;
        }

        private void OSM_Map_OnDoubleClick()
        {
            var newzoom = zoom / 2.0;
            Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMouseLogicalPos);
            this.OSM_Map.ZoomAboutLogicalPoint(zoom / newzoom, logicalPoint.X, logicalPoint.Y);
            zoom = newzoom;
        }

        private void OSM_Map_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (duringDrag)
            {
                Point newPoint = lastMouseViewPort;
                Point thisMouseLogicalPos = e.GetPosition(this.OSM_Map);
                newPoint.X += (lastMouseLogicalPos.X - thisMouseLogicalPos.X) / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                newPoint.Y += (lastMouseLogicalPos.Y - thisMouseLogicalPos.Y) / this.OSM_Map.ActualWidth * this.OSM_Map.ViewportWidth;
                this.OSM_Map.ViewportOrigin = newPoint;
            }
        }
    }
}
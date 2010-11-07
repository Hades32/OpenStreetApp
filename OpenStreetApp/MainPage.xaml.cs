using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        Point lastMousePos = new Point();
        Point lastMouseLogicalPos = new Point();
        Point lastMouseViewPort = new Point();
        bool duringDrag = false;
        double zoom = 1;

        public MainPage()
        {
            InitializeComponent();

            this.OSM_Map.Source = new OSMTileSource();

            var down = Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.OSM_Map, "MouseLeftButtonDown");
            var up = Microsoft.Phone.Reactive.Observable.FromEvent<MouseButtonEventArgs>(this.OSM_Map, "MouseLeftButtonUp");
            var downAndUp = down.Zip(up, (handler, event_arg) => event_arg);

            // das funzt noch nicht! (richtig)
            var x = downAndUp.Timestamp().BufferWithCount(2).SubscribeOnDispatcher().Subscribe(
                eventList =>
                {
                    // zoom if it was actually a double click and not just two clicks
                    if (eventList[1].Timestamp - eventList[0].Timestamp < TimeSpan.FromSeconds(2))
                    {
                        var newzoom = zoom / 1.3;
                        Point logicalPoint = this.OSM_Map.ElementToLogicalPoint(this.lastMousePos);
                        this.OSM_Map.ZoomAboutLogicalPoint(zoom / newzoom, logicalPoint.X, logicalPoint.Y);
                        zoom = newzoom;
                    }
                });
        }

        public void TestTiles(Point coord, double zoom, string path)
        {
            //this.picture.Source = new BitmapImage(new Uri(path));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*TileDownloadManager tdm = new TileDownloadManager();
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(5000);
                Point p = new Point(46.8 + (i * 3), 10.1 + (i * 5));
                tdm.fetch(p, 12, new Action<Point,double,string>(TestTiles));
            }*/
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
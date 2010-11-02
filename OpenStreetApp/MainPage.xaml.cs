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
using System.Windows.Media.Imaging;

namespace OpenStreetApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
        }

        public void TestTiles(Point coord, double zoom, String path)
        {
            this.picture.Source = new BitmapImage(path);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            TileDownloadManager tdm = new TileDownloadManager();
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(5000);
                Point p = new Point(46.8 + (i * 3), 10.1 + (i * 5));
                tdm.fetch(p, 12, new Action<Point,double,String>(TestTiles));
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {

        }
    }
}
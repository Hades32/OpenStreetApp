using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace OpenStreetApp
{
    public partial class App : Application
    {
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static System.Exception LastException { get; set; }

        public static GeoCoordinateWatcher watcher;
        public static GeoPosition<GeoCoordinate> lastKnownPosition;

        /// <summary>
        /// stores results from pages. The string from the pair is used to store calling site information.
        /// </summary>
        public static Dictionary<Type, KeyValuePair<string, object>> NavigationResults
            = new Dictionary<Type, KeyValuePair<string, object>>();

        public App()
        {
            UnhandledException += Application_UnhandledException;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Zähler für die aktuelle Bildrate anzeigen.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Bereiche der Anwendung hervorheben, die mit jedem Bild neu gezeichnet werden.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Nicht produktiven Visualisierungsmodus für die Analyse aktivieren, 
                // in dem GPU-beschleunigte Bereiche der Seite farbig hervorgehoben werden.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            InitializeComponent();

            InitializePhoneApplication();

            // Initialize GeoLocation Listener
            watcher = new GeoCoordinateWatcher();
            watcher.MovementThreshold = 20;

            watcher.StatusChanged += watcher_StatusChanged;
            watcher.PositionChanged += watcher_PositionChanged;

            watcher.Start();
            System.Diagnostics.Debug.WriteLine("watcher started");
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            System.Diagnostics.Debug.WriteLine("receive coordinate, now setting");
            lastKnownPosition = e.Position;
            System.Diagnostics.Debug.WriteLine("new coordinate is now:" + lastKnownPosition.Location.Latitude + ", " + lastKnownPosition.Location.Longitude);
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // READ ARTICLE FOR POPUP - ERROR Message
                    break;
                case GeoPositionStatus.Ready:
                    System.Diagnostics.Debug.WriteLine("service is now ready");
                    break;

                default:
                    break;
            }
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                e.Handled = true;
                App.LastException = e.ExceptionObject;
                if (!this.RootFrame.Navigate(
                    new System.Uri("/ErrorPage.xaml", System.UriKind.Relative)))
                    e.Handled = false;
            }
        }

        #region Initialisierung der Phone-Anwendung

        private bool phoneApplicationInitialized = false;

        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            this.RootVisual = new LoadingPage();

            OSA_Configuration.Instance.initialize(() =>
                {
                    RootFrame = new PhoneApplicationFrame();
                    RootFrame.Navigated += CompleteInitializePhoneApplication;

                    RootFrame.NavigationFailed += RootFrame_NavigationFailed;

                    phoneApplicationInitialized = true;

                }, this.RootVisual.Dispatcher);
        }

        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            if (this.RootVisual is LoadingPage)
                ((LoadingPage)this.RootVisual).progress.IsIndeterminate = false;

            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
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
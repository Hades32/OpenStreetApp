using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class PreferencesPage : PhoneApplicationPage
    {
        public PreferencesPage()
        {
            InitializeComponent();
            this.DataContext = OSA_Configuration.Instance;
        }

        private void toggleSwitch1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch1.Content = "Current Position";
        }

        private void toggleSwitch1_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch1.Content = "World View";
        }

        private void toggleSwitch2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch2.Content = "Yes";
        }

        private void toggleSwitch2_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch2.Content = "No";
        }

        private void toggleSwitch3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch3.Content = "Yes";
        }

        private void toggleSwitch3_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch3.Content = "No";
        } 
    }
}
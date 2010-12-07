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
            this.toggleSwitch1.Content = "Aktuelle Position";
        }

        private void toggleSwitch1_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.toggleSwitch1.Content = "Weltkarte";
        } 
    }
}
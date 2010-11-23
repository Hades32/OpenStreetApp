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
    }
}
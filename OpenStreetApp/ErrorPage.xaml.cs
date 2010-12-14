using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace OpenStreetApp
{
    public partial class ErrorPage : PhoneApplicationPage
    {
        public ErrorPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.LastException != null)
            {
                this.ErrorTB.Text = App.LastException.ToString();
            }
            else if (this.State.ContainsKey("error"))
            {
                this.ErrorTB.Text = this.State["error"].ToString();
            }
            else
            {
                this.ErrorTB.Text = "No error? This is an error!";
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.State.setOrAdd("error", this.ErrorTB.Text);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            e.Cancel = true;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var nl = System.Environment.NewLine;
            var mail = new Microsoft.Phone.Tasks.EmailComposeTask();
            mail.To = "wp7@rauscheronline.de";
            mail.Subject = "error in Open Street App";
            mail.Body = "An error happened in OSA when I did the following:" + nl + nl +
                        "The error message is this:" + nl
                        + this.ErrorTB.Text;
            mail.Show();
        }
    }
}
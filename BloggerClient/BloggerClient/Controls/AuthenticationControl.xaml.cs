using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Blogger;
using System.Text.RegularExpressions;
using System.Text;

namespace BloggerClient.Controls
{
    public partial class AuthenticationControl : UserControl
    {
        #region Properties
        public string Login { get { return LoginBox.Text; } }
        public string AuthorizationCode { get { return AuthBox.Text; } }
        #endregion

        public AuthenticationControl()
        {
            InitializeComponent();            
        }

        private void AuthorizeButton_Click_1(object sender, RoutedEventArgs e)
        {
            Communicator.Instance.Login = LoginBox.Text;
            Communicator.Instance.AuthorizationToken = AuthBox.Text;
            Communicator.Instance.LoadAccessTokens();
        }

        private void GetAuthCodeButton_Click_1(object sender, RoutedEventArgs e)
        {
            AuthProxyBrowser.Visibility = Visibility.Visible;
            var uri = new Uri("https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=715063550855.apps.googleusercontent.com&redirect_uri=urn:ietf:wg:oauth:2.0:oob&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fblogger&login_hint=asm@exanple.com", UriKind.Absolute);
            AuthProxyBrowser.Navigate(uri);
        }

        private void BrowserFrame_LoadCompleted_1(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Communicator.Instance.AuthorizationToken = string.Empty;
            var source = AuthProxyBrowser.SaveToString();
            var regex = new Regex("code=(.+?)<");
            var match = regex.Matches(source).OfType<Match>().FirstOrDefault();

            if (match != null)
            {
                AuthBox.Text = match.Groups.Count > 1 ? match.Groups[1].ToString() : string.Empty;
                AuthProxyBrowser.Visibility = Visibility.Collapsed;
            }
        }
    }
}

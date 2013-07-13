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

using Blogger.Core;
using System.Text.RegularExpressions;
using System.Text;
using System.IO.IsolatedStorage;

namespace Blogger.UI.Controls
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
            var uri = new Uri(string.Format("{0}?response_type=code&client_id={1}&redirect_uri={2}&scope={3}&login_hint={4}", 
                                            Communicator.Instance.AuthorizationRequestUrl,
                                            Communicator.Instance.ClientId,
                                            Communicator.Instance.RedirectUri,
                                            Communicator.Instance.PermissionsScope,
                                            Communicator.Instance.Login), 
                              UriKind.Absolute);
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

        private void LoginBox_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = Communicator.Instance.Login;
        }

        private void LoginBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            // TODO: Add checking
            Communicator.Instance.Login = LoginBox.Text;
        }
    }
}

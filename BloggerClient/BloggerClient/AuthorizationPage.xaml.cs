using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BloggerClient.Pages
{
    public partial class AuthorizationPage : PhoneApplicationPage
    {
        public AuthorizationPage()
        {
            InitializeComponent();
            NavigationService.Navigate(new Uri(""));
        }
    }
}
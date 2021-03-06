﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Blogger.ViewModels;

namespace BloggerClient.Pages
{
    public partial class AuthorizationPage : PhoneApplicationPage
    {
        public AuthorizationPageViewModel ViewModel { get; private set; }

        public AuthorizationPage()
        {
            InitializeComponent();

            DataContext = ViewModel = new AuthorizationPageViewModel();
            ViewModel.AuthorizeCommandExecute = p =>
                {
                    NavigationService.Navigate(new Uri("/Main", UriKind.Relative));
                };
        }
    }
}
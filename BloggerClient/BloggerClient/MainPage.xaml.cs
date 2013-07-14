using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;
using Blogger;
using Blogger.ViewModels;

namespace Blogger.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Proverties

        public MainPageViewModel ViewModel { get; private set; }

        #endregion

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = ViewModel = new MainPageViewModel();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CheckTokens(
                // onTokensExpired
                () => 
                {
                    throw new NotFiniteNumberException();
                },

                // onTokensOk
                () => 
                {
                    throw new NotFiniteNumberException();
                },

                // onConnectionError
                () => 
                {
                    throw new NotFiniteNumberException();
                });
        }
    }
}
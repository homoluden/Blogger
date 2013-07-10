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

namespace BloggerClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        //private void BrowserFrame_LoadCompleted_1(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    Communicator.Instance.AuthorizationToken = string.Empty;
        //    var source = BrowserFrame.SaveToString();
        //    var regex = new Regex("id=\"code\"[\\w\\s\\\"\\=\\.\\(\\);]+?value=\"(.+?)\"");
        //    var match = regex.Matches(source).OfType<Match>().FirstOrDefault();

        //    if (match != null)
        //    {
        //        Communicator.Instance.AuthorizationToken = match.Groups.Count > 1 ? match.Groups[1].ToString() : string.Empty;
        //        Communicator.Instance.LoadAccessTokens();
        //    }
        //}
    }
}
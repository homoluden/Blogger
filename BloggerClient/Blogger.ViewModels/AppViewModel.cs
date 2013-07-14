using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Blogger.Strings;
using Blogger.Enums;
using Blogger.Core;


namespace Blogger.ViewModels
{
    public class AppViewModel : BaseViewModel
    {

        #region Public Methods

        public void SaveSettings()
        {
            Communicator.Instance.SaveSettings();
        }

        #endregion

        public AppViewModel()
        {
            Communicator.Instance.LoadSettings();
        }        
    }
}
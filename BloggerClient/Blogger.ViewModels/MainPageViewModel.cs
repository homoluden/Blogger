using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace Blogger.ViewModels
{
    public class MainPageViewModel
    {
        #region Private Methods

        private void LoadSettings()
        { 
            var settings = IsolatedStorageSettings.ApplicationSettings;
            Communicator.Instance.Login = settings.Contains
        }

        #endregion
    }
}

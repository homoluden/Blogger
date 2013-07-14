using Blogger.Strings;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Blogger.Core;
using System.Windows;
using System.Windows.Navigation;

namespace Blogger.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Fields
        private bool _tokensOk;
        #endregion


        #region Properties
        public bool IsTokensOk 
        {
            get { return _tokensOk; }
            set 
            {
                _tokensOk = value;
                RaisePropertyChanged("IsTokensOk");
            }
        }
        #endregion


        #region Public Methods
        public void CheckTokens(Action onTokensExpired, Action onTokensOk, Action onConnectionError)
        {
            if (string.IsNullOrWhiteSpace(Communicator.Instance.AuthorizationToken) ||
                string.IsNullOrWhiteSpace(Communicator.Instance.AccessToken) ||
                string.IsNullOrWhiteSpace(Communicator.Instance.RefreshToken))
            {
                if (onTokensExpired != null)
                {
                    onTokensExpired();
                }
                return;
            }

            Communicator.Instance.LoadUserInfoAsync();
        }
        #endregion


        #region Private Methods

        #endregion


        #region Ctors
        public MainPageViewModel()
        { 
            
        }
        #endregion
    }
}

using Blogger.Strings;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Blogger.Core;
using System.Windows;
using System.Windows.Navigation;
using Blogger.Utils;
using Blogger.Data.Responses;

namespace Blogger.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Fields

        private bool _tokensOk = false;
        
        private bool _connectionError = false;
        
        #endregion


        #region Commands

        public RelayCommand<object> CheckTokensCommand { get; private set; }  
 
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

        public bool IsConnectionErrorAppeared
        {
            get { return _connectionError; }
            set 
            {
                _connectionError = value;
                RaisePropertyChanged("IsConnectionErrorAppeared");
            }
        }

        #endregion


        #region Public Methods

        public void CheckTokens(Action needRefreshTokensCallback, Action tokensOkCallback, Action connectionErrorCallback)
        {
            if (Communicator.Instance.AuthorizationToken == StringsManagers.NOT_AVAILABLE_STRING ||
                Communicator.Instance.AccessToken == StringsManagers.NOT_AVAILABLE_STRING ||
                Communicator.Instance.RefreshToken == StringsManagers.NOT_AVAILABLE_STRING)
            {
                if (needRefreshTokensCallback != null)
                {
                    needRefreshTokensCallback();
                }
                return;
            }

            Communicator.Instance.LoadUserInfoAsync(UserInfoLoadingSuccessCallback, UserInfoLoadingErrorCallback);
        }

        #endregion


        #region Private Methods

        private void UserInfoLoadingErrorCallback(ErrorResponse result)
        {
            //TODO: Log error here. User Info loading failed.
        }

        private void UserInfoLoadingSuccessCallback(UserInfoResponse result)
        {
            Locator.Instance.RootViewModel.UserInfo = result;
        }

        private void RefreshTokensCallback()
        {
            IsTokensOk = false;
        }
        
        private void TokensOkCallback()
        {
            IsTokensOk = true;
        }

        private void ConnectionErrorCalback()
        {
            IsConnectionErrorAppeared = true;
        }

        #endregion


        #region Ctors
        public MainPageViewModel()
        { 
            CheckTokensCommand = new RelayCommand<object>(p => CheckTokens(RefreshTokensCallback, TokensOkCallback, ConnectionErrorCalback), null);
        }
        #endregion
    }
}

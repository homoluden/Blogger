using Blogger.Data.Responses;
using Blogger.Enums;
using Blogger.Strings;
using ServiceStack.Text;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Blogger.Core
{
    public sealed class Communicator
    {
        #region Fields
        
        private IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;

        private HttpClientHandler _handler;
        private HttpClient _client;

        #endregion


        #region Properties

        public string Login { get; set; }
        
        public string AuthorizationToken { get; set; }
        
        public string AccessToken { get; set; }
        
        public string RefreshToken { get; set; }
        
        public bool IsAuthorized { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string TokenRequestUrl { get; set; }

        public string AuthorizationRequestUrl { get; set; }

        public string PermissionsScope { get; set; }

        #endregion

        
        #region Private Methods
        
        private string TryLoadIndividualSetting(IsolatedStorageSettings settings, Setting settingNameEnum)
        {
            string settingValue;

            return settings.TryGetValue<string>(StringsManagers.GetSettingName(settingNameEnum), out settingValue) ?
                                            settingValue :
                                            StringsManagers.GetSettingDefaultValue(settingNameEnum);
        }

        public void LoadSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            
            Communicator.Instance.Login = TryLoadIndividualSetting(settings, Setting.LoginSettingName);

            Communicator.Instance.AuthorizationToken = TryLoadIndividualSetting(settings, Setting.AuthTokenSettingName);

            Communicator.Instance.AccessToken = TryLoadIndividualSetting(settings, Setting.AccessTokenSettingName);

            Communicator.Instance.RefreshToken = TryLoadIndividualSetting(settings, Setting.RefreshTokenSettingName);
            
            Communicator.Instance.ClientId = TryLoadIndividualSetting(settings, Setting.ClientIdSettingName);

            Communicator.Instance.ClientSecret = TryLoadIndividualSetting(settings, Setting.ClientSecretSettingName);

            Communicator.Instance.RedirectUri = TryLoadIndividualSetting(settings, Setting.RedirectUriSettingName);

            Communicator.Instance.TokenRequestUrl = TryLoadIndividualSetting(settings, Setting.TokenRequestUrlSettingName);

            Communicator.Instance.AuthorizationRequestUrl = TryLoadIndividualSetting(settings, Setting.AuthorizationRequestUrlSettingName);

            Communicator.Instance.PermissionsScope = TryLoadIndividualSetting(settings, Setting.PermissionsScopeSettingName);
        }

        private void TrySetIndividualSetting(IsolatedStorageSettings settings, Setting settingNameEnum, string settingValue)
        {
            var settingName = settingNameEnum.ToString();
            if (settings.Contains(settingName))
            {
                settings[settingName] = settingValue;
            }
            else
            {
                settings.Add(settingName, settingValue);
            }            
        }

        public void SaveSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            TrySetIndividualSetting(settings, Setting.LoginSettingName, Communicator.Instance.Login);

            TrySetIndividualSetting(settings, Setting.AuthTokenSettingName, Communicator.Instance.AuthorizationToken);

            TrySetIndividualSetting(settings, Setting.AccessTokenSettingName, Communicator.Instance.AccessToken);

            TrySetIndividualSetting(settings, Setting.RefreshTokenSettingName, Communicator.Instance.RefreshToken);

            TrySetIndividualSetting(settings, Setting.ClientIdSettingName, Communicator.Instance.ClientId);

            TrySetIndividualSetting(settings, Setting.ClientSecretSettingName, Communicator.Instance.ClientSecret);

            TrySetIndividualSetting(settings, Setting.RedirectUriSettingName, Communicator.Instance.RedirectUri);

            TrySetIndividualSetting(settings, Setting.TokenRequestUrlSettingName, Communicator.Instance.TokenRequestUrl);

            TrySetIndividualSetting(settings, Setting.AuthorizationRequestUrlSettingName, Communicator.Instance.AuthorizationRequestUrl);

            TrySetIndividualSetting(settings, Setting.PermissionsScopeSettingName, Communicator.Instance.PermissionsScope);

            settings.Save();
        }

        private object HandleAccessTokensResponse(Task<HttpResponseMessage> task) 
        {
            var readTask = task.Result.Content.ReadAsStringAsync();
            readTask.Wait();

            var response = readTask.Result;
            
            return response.FromJson<AccessTokensResponse>();
        }
        private void HandleAccessTokenLoadingError(Task<HttpResponseMessage> task) 
        {
            // TODO: Handle error here
        }
        #endregion

        #region Private Methods
        private void InitializeCommunicator() 
        {
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler);
        }
        #endregion

        #region Public Methods
        public void LoadAccessTokens() 
        {
            if (!IsAuthorized)
            {
                return;
            }

            var contentString = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", 
                                              AuthorizationToken,
                                              ClientId,
                                              ClientSecret,
                                              RedirectUri);

            var content = new StringContent(contentString);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                                    
            var postTask = _client.PostAsync(TokenRequestUrl, content);
            postTask.
                ContinueWith(t =>
                    {
                        content.Dispose();
                        //writer.Dispose(); // Disposed automagically after content.Dispose() - IDisposable convention
                        //stream.Dispose(); // Disposed automagically after content.Dispose() - IDisposable convention

                        if (t.IsFaulted)
                        {
                            HandleAccessTokenLoadingError(t);
                        }
                        else
                        {
                            HandleAccessTokensResponse(t);
                        }
                    }
                );
        }
        #endregion


        #region Singleton impl.
        private static volatile Communicator instance;
        private static object syncRoot = new Object();

        private Communicator() 
        {
            InitializeCommunicator();
        }

        public static Communicator Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null)
                            instance = new Communicator();
                    }
                }

                return instance;
            }
        }
        #endregion
    }
}

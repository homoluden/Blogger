using Blogger.Data.Responses;
using Blogger.Enums;
using Blogger.Resources;
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

            return settings.TryGetValue<string>(StringsManager.GetSettingName(settingNameEnum), out settingValue) ?
                                            settingValue :
                                            StringsManager.NOT_AVAILABLE_STRING;
        }

        private void LoadSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            
            Communicator.Instance.Login = TryLoadIndividualSetting(settings, Setting.LoginName);

            Communicator.Instance.AuthorizationToken = TryLoadIndividualSetting(settings, Setting.AuthTokenName);

            Communicator.Instance.AccessToken = TryLoadIndividualSetting(settings, Setting.AccessTokenName);

            Communicator.Instance.RefreshToken = TryLoadIndividualSetting(settings, Setting.RefreshTokenName);
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

        private void SaveSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            TrySetIndividualSetting(settings, Setting.LoginName, Communicator.Instance.Login);

            TrySetIndividualSetting(settings, Setting.AuthTokenName, Communicator.Instance.AuthorizationToken);

            TrySetIndividualSetting(settings, Setting.AccessTokenName, Communicator.Instance.AccessToken);

            TrySetIndividualSetting(settings, Setting.RefreshTokenName, Communicator.Instance.RefreshToken);

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
            if (!_settings.Contains("Login"))
            {
                Login = "asm@example.com";
            }

            if (!_settings.Contains("ClientId"))
            {
                ClientId = "715063550855.apps.googleusercontent.com";
            }

            if (!_settings.Contains("ClientSecret"))
            {
                ClientSecret = "iquSk4i-DHCXdYYZdLGPmMzJ";
            }

            if (!_settings.Contains("RedirectUri"))
            {
                RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
            }

            if (!_settings.Contains("TokenRequestUrl"))
            {
                TokenRequestUrl = "https://accounts.google.com/o/oauth2/token";
            }

            if (!_settings.Contains("AuthorizationRequestUrl"))
            {
                AuthorizationRequestUrl = "https://accounts.google.com/o/oauth2/auth";
            }

            if (!_settings.Contains("PermissionsScope"))
            {
                PermissionsScope = "https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fblogger";
            }

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

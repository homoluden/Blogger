using Blogger.Responses;
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

namespace Blogger
{
    public sealed class Communicator
    {
        #region Fields
        private IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;

        private string _login = string.Empty;
        private string _authToken = string.Empty;

        private bool _isAuthorized;
        private HttpClientHandler _handler;
        private HttpClient _client;
        #endregion


        #region Properties
        public string Login
        {
            get { return (string)_settings["Login"]; }
            set 
            {
                _settings["Login"] = value;
            }
        }
        
        public string AuthorizationToken
        {
            get { return (string)_settings["AuthToken"]; }
            set 
            {
                _settings["AuthToken"] = value;
                _isAuthorized = !string.IsNullOrWhiteSpace(value);
            }
        }
        public bool IsAuthorized { get { return _isAuthorized; } }

        public string ClientId 
        { 
            get { return (string)_settings["ClientId"]; }
            set { _settings["ClientId"] = value; }
        }

        public string ClientSecret 
        {
            get { return (string)_settings["ClientSecret"]; }
            set { _settings["ClientSecret"] = value; }
        }

        public string RedirectUri 
        {
            get { return (string)_settings["RedirectUri"]; }
            set { _settings["RedirectUri"] = value; }
        }

        public string TokenRequestUrl 
        {
            get { return (string)_settings["TokenRequestUrl"]; }
            set { _settings["TokenRequestUrl"] = value; }
        }

        public string AuthorizationRequestUrl 
        {
            get { return (string)_settings["AuthorizationRequestUrl"]; }
            set { _settings["AuthorizationRequestUrl"] = value; }
        }

        public string PermissionsScope 
        {
            get { return (string)_settings["PermissionsScope"]; }
            set { _settings["PermissionsScope"] = value; }
        }

        #endregion


        #region Private Methods
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

using Google.GData.Blogger;
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
            get { return _settings["Login"].ToString(); }
            set 
            {
                _settings["Login"] = value;
            }
        }
        
        public string AuthorizationToken
        {
            get { return _settings["AuthToken"].ToString(); }
            set 
            {
                _settings["AuthToken"] = value;
                _isAuthorized = !string.IsNullOrWhiteSpace(value);
            }
        }
        public bool IsAuthorized { get { return _isAuthorized; } }

        public string ClientId 
        { 
            get { return _settings["ClientId"].ToString(); }
            set { _settings["ClientId"] = value; }
        }

        public string ClientSecret 
        {
            get { return _settings["ClientSecret"].ToString(); }
            set { _settings["ClientSecret"] = value; }
        }

        public string RedirectUri 
        {
            get { return _settings["RedirectUri"].ToString(); }
            set { _settings["RedirectUri"] = value; }
        }

        public string TokenRequestUrl 
        {
            get 
            {
                var requestUrl = _settings["TokenRequestUrl"].ToString();
                if (string.IsNullOrWhiteSpace(requestUrl))
                {
                    _settings["TokenRequestUrl"] = requestUrl = "https://accounts.google.com/o/oauth2/token";
                }
                return requestUrl; 
            }            
        }

        public string AuthorizationRequestUrl 
        {
            get 
            {
                var requestUrl = _settings["AuthorizationRequestUrl"].ToString();
                if (string.IsNullOrWhiteSpace(requestUrl))
                {
                    _settings["AuthorizationRequestUrl"] = requestUrl = "https://accounts.google.com/o/oauth2/auth";
                }
                return requestUrl; 
            }            
        }

        public string PermissionsScope 
        {
            get 
            {
                var requestUrl = _settings["PermissionsScope"].ToString();
                if (string.IsNullOrWhiteSpace(requestUrl))
                {
                    _settings["PermissionsScope"] = requestUrl = "https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fblogger";
                }
                return requestUrl; 
            }            
        }

        #endregion


        #region Private Methods
        private object HandleAccessTokensResponse(Task<HttpResponseMessage> task) 
        {
            var readTask = task.Result.Content.ReadAsStringAsync();
            readTask.Wait();

            var response = readTask.Result;

            return response;
        }
        private void HandleAccessTokenLoadingError(Task<HttpResponseMessage> task) 
        {
            // TODO: Handle error here
        }
        #endregion

        #region Private Methods
        private void InitializeCommunicator() 
        {
            if (ClientId == null)
            {
                ClientId = "715063550855.apps.googleusercontent.com";
            }

            if (ClientSecret == null)
            {
                ClientSecret = "iquSk4i-DHCXdYYZdLGPmMzJ";
            }

            if (RedirectUri == null)
            {
                RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
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
                                              _authToken,
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

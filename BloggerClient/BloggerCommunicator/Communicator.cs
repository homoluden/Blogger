using Google.GData.Blogger;
using System;
using System.IO;
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
        private string _login = string.Empty;
        private string _authToken = string.Empty;

        private bool _isAuthorized;
        private HttpClientHandler _handler;
        private HttpClient _client;
        #endregion


        #region Properties
        public string Login
        {
            get { return _login; }
            set 
            { 
                _login = value;                
            }
        }
        
        public string AuthorizationToken
        {
            get { return _authToken; }
            set 
            { 
                _authToken = value;
                _isAuthorized = !string.IsNullOrWhiteSpace(value);
            }
        }
        public bool IsAuthorized { get { return _isAuthorized; } }
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



        #region Public Methods
        public void LoadAccessTokens() 
        {
            if (!IsAuthorized)
            {
                return;
            }

            var contentString = string.Format("code={0}&client_id=715063550855.apps.googleusercontent.com&client_secret=iquSk4i-DHCXdYYZdLGPmMzJ&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code", _authToken);
            var content = new StringContent(contentString);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                                    
            var postTask = _client.PostAsync("https://accounts.google.com/o/oauth2/token", content);
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
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler);            
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

using Blogger.Data.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Blogger.Core.Services
{
    public class RestClient
    {
        #region Fields
        HttpClientHandler _handler;
        HttpClient _client;
        #endregion

        #region Public Methods
        
        public void LoadAccessTokensAsync(Action<Task<HttpResponseMessage>> onSuccess, Action<Task<HttpResponseMessage>> onError)
        {
            if (string.IsNullOrWhiteSpace(Communicator.Instance.AuthorizationToken))
            {
                return;
            }

            var contentString = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code",
                                              Communicator.Instance.AuthorizationToken,
                                              Communicator.Instance.ClientId,
                                              Communicator.Instance.ClientSecret,
                                              Communicator.Instance.RedirectUri);

            var content = new StringContent(contentString);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var postTask = _client.PostAsync(Communicator.Instance.TokenRequestUrl, content);
            postTask.
                ContinueWith(t =>
                {
                    content.Dispose();
                    
                    if (t.IsFaulted)
                    {
                        if (onError != null)
                        {
                            onError(t);
                        }                        
                    }
                    else
                    {
                        var readTask = t.Result.Content.ReadAsStringAsync();
                        readTask.Wait();

                        var response = readTask.Result.FromJson<AccessTokensResponse>();

                        Communicator.Instance.AccessToken = response.access_token;
                        Communicator.Instance.RefreshToken = response.refresh_token;
                        Communicator.Instance.ExpiresIn = response.expires_in;

                        if (onSuccess != null)
                        {
                            onSuccess(t);
                        }                        
                    }
                });
        }

        public void LoadUserInfoAsync(Action<Task<HttpResponseMessage>> onSuccess, Action<Task<HttpResponseMessage>> onError)
        {
            if (string.IsNullOrWhiteSpace(Communicator.Instance.AccessToken))
            {
                return;
            }

            var queryString = string.Format("?access_token={0}", Communicator.Instance.AccessToken);

            var getTask = _client.GetAsync(Communicator.Instance.UserInfoRequestUrl);
            getTask.
                ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        if (onError != null)
                        {
                            onError(t);
                        }
                    }
                    else
                    {
                        var readTask = t.Result.Content.ReadAsStringAsync();
                        readTask.Wait();

                        var response = readTask.Result.FromJson<AccessTokensResponse>();

                        Communicator.Instance.AccessToken = response.access_token;
                        Communicator.Instance.RefreshToken = response.refresh_token;
                        Communicator.Instance.ExpiresIn = response.expires_in;

                        if (onSuccess != null)
                        {
                            onSuccess(t);
                        }
                    }
                });
        }
        #endregion


        #region Private Methods

        private void HandleAccessTokenLoadingError(Task<HttpResponseMessage> task)
        {
            // TODO: Handle error here
        }

        #endregion

        #region Ctors
        public RestClient()
        {
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler);
        }
        #endregion
    }
}

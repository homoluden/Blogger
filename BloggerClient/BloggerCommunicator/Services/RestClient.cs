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

        #region Static Methods

        public static T ParseJson<T>(HttpResponseMessage message) 
        {
            var readTask = message.Content.ReadAsStringAsync();
            readTask.Wait();

            var response = readTask.Result.FromJson<T>();

            return response;
        }

        #endregion

        #region Public Methods

        public void LoadAccessTokensAsync(Action<AccessTokensResponse> onSuccess, Action<ErrorResponse> onError)
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
                            onError(new ErrorResponse() { Error = t.Exception, RawResult = ParseJson<ServerErrorResponse>(t.Result)});
                        }                        
                    }
                    else
                    {
                        var response = ParseJson<AccessTokensResponse>(t.Result);

                        //Communicator.Instance.AccessToken = response.access_token;
                        //Communicator.Instance.RefreshToken = response.refresh_token;
                        //Communicator.Instance.ExpiresIn = response.expires_in;

                        //TODO: Check response content here. Invoke error callback if response doesn't parse well.

                        if (onSuccess != null)
                        {
                            onSuccess(response);
                        }                        
                    }
                });
        }

        public void LoadUserInfoAsync(Action<UserInfoResponse> onSuccess, Action<ErrorResponse> onError)
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
                            onError(new ErrorResponse() { Error = t.Exception, RawResult = ParseJson<ServerErrorResponse>(t.Result) });
                        }
                    }
                    else
                    {
                        var response = ParseJson<UserInfoResponse>(t.Result);

                        //TODO: Check response content here. Invoke error callback if response doesn't parse well.

                        if (onSuccess != null)
                        {
                            onSuccess(response);
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

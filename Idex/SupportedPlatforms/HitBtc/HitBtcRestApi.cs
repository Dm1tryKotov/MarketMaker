using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using MMS.SupportedPlatforms.HitBtc.Model;
using MMS.SupportedPlatforms.HitBtc.Categories;

namespace MMS.SupportedPlatforms.HitBtc
{
    public class HitBtcRestApi
    {
        private const string URL = "https://api.hitbtc.com/api/2";

        private string _pubkey { get; set; }
        private string _secretkey { get; set; }
        private bool isAuthenticated;

        public PublicData PublicData { get; set; }
        public Trade Trade { get; set; }
        public Account Account { get; set; }

        public HitBtcRestApi()
        {
            PublicData = new PublicData(this);
            Trade = new Trade(this);
            Account = new Account(this);
        }

        public void Auth(string pub, string secret)
        {
            _pubkey = pub;
            _secretkey = secret;
            isAuthenticated = true;
        }

        public async Task<ApiResponse> Execute(RestRequest request, bool requireAuth = false)
        {
            var client = new RestClient(URL);

            if (requireAuth && isAuthenticated)
            {
                client.Authenticator = new HttpBasicAuthenticator(_pubkey, _secretkey);
            }

            var response = await client.GetResponseAsync(request).ConfigureAwait(false);

            if (response.ErrorException != null)
            {
                ExceptionInfo exceptionInfo = new ExceptionInfo {
                    ExceptionDetail = new ExceptionDetail {
                        FullInfo = response.ErrorException.ToString(),
                        Message = response.ErrorException.Message
                    }
                };

                return new ApiResponse { Content = Utilities.Serialize(exceptionInfo) };
            }
            
            return response.Content.StartsWith("[")
                ? new ApiResponse { Content = $"{{\"collections\":{response.Content }}}"}
                : new ApiResponse { Content = response.Content };

        }
    }

    public static class RestClientExtensions
    {
        private static Task<T> SelectAsync<T>(this RestClient client, IRestRequest request,
            Func<IRestResponse, T> selector)
        {
            var tcs = new TaskCompletionSource<T>();
            var loginResponse = client.ExecuteAsync(request, r =>
            {
                tcs.SetResult(selector(r));
            });
            return tcs.Task;
        }

        public static Task<IRestResponse> GetResponseAsync(this RestClient client, IRestRequest request)
        {
            return client.SelectAsync(request, r => r);
        }
    }
}

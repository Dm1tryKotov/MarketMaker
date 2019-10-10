using SupportedPlatforms.Idex.Categories;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportedPlatforms.Idex
{
    public class IdexRestApi
    {
        private const string Url = "https://api.idex.market";
        private bool notConnection = false;

        public string Address { get; set; }
        public string PrivateKey { get; set; }

        public PublicData publicData { get; set; }

        public IdexRestApi()
        {
            publicData = new PublicData(this);
        }

        public void Auth(string address, string privateKey)
        {
            Address = address;
            PrivateKey = privateKey;
        }

        public async Task<ApiResponse> Execute(RestRequest request)
        {
            if (notConnection)
            {
                Task.Delay(10 * 1000).Wait();
            }

            var client = new RestClient(Url);

            var response = await client.GetResponseAsync(request).ConfigureAwait(false);
            
            if (response.ErrorException != null)
            {
                if (response.ErrorException.Message == "Невозможно разрешить удаленное имя: 'api.idex.market'")
                {
                    notConnection = true;
                }
                else
                {
                    notConnection = false;
                }
                return new ApiResponse { Content = "{\"exception\":\"" + response.ErrorException.Message + "\"}" };
            }

            return response.StatusCode != System.Net.HttpStatusCode.OK ? new ApiResponse { Content = "{\"status_code\":\"" + response.StatusCode + ", message" + response.Content + "\"}" } : new ApiResponse { Content = response.Content };

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

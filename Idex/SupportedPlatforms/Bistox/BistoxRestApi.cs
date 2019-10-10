using MMS.SupportedPlatforms.Bistox.Categories;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox
{
    public class BistoxRestApi
    {
        private const string URL = "http://13.233.23.179:14001";

        private string _pubkey { get; set; }
        private string _secretkey { get; set; }
        private bool isAuthenticated;

        public PublicData PublicData { get; set; }

        public BistoxRestApi()
        {
            PublicData = new PublicData(this);
        }

        public async Task<ApiResponse> Execute(RestRequest request, bool requireAuth = false)
        {
            var client = new RestClient(URL);

            var response = await client.GetResponseAsync(request).ConfigureAwait(false);

            if (response.ErrorException != null)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Content.StartsWith("[")
                ? new ApiResponse { Content = $"{{\"collections\":{response.Content }}}" }
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

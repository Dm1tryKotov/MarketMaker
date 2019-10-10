using RestSharp;
using MMS.SupportedPlatforms.HitBtc.Model;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Categories
{
    public class Account
    {
        private HitBtcRestApi _api;

        public Account(HitBtcRestApi api)
        {
            _api = api;
        }

        public async Task<Balances> GetBalance()
        {
            return await _api.Execute(new RestRequest("/trading/balance", Method.GET), true);
        }

        public async Task<Orders> GetOrders(string symbol)
        {
            return await _api.Execute(new RestRequest($"/order?{symbol}", Method.GET), true);
        }
    }
}

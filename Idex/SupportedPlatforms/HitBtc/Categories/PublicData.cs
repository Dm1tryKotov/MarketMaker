using MMS.SupportedPlatforms.HitBtc.Model;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Categories
{
    public class PublicData
    {
        private HitBtcRestApi _api;

        public PublicData(HitBtcRestApi api)
        {
            _api = api;
        }

        public async Task<OrderBook> getOrderBook(string pairName)
        {
            return await _api.Execute(new RestRequest($"/public/orderbook/{pairName}", Method.GET));
        }

        public async Task<Candles> getCandles(string pairName, int limit, string timeFrame)
        {
            return await _api.Execute(new RestRequest($"public/candles/{pairName}?period={timeFrame}&limit={limit}", Method.GET));
        }

        public async Task<Symbols> getSymbols()
        {
            return await _api.Execute(new RestRequest($"public/symbol", Method.GET));
        }
       
    }
}

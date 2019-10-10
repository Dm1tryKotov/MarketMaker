using SupportedPlatforms.Idex.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportedPlatforms.Idex.Categories
{
    public class PublicData
    {
        private readonly IdexRestApi _api;
        public PublicData(IdexRestApi api) {
            _api = api;
        }

        public async Task<Balance> getBalance() {
            var request = new RestRequest("/returnBalance", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", $"{{\n\t\"address\": \"{_api.Address}\"\n}}", ParameterType.RequestBody);
            return await _api.Execute(request);
        }

        public async Task<DailyVolume> get24Volume() {
            var request = new RestRequest("/return24Volume", Method.GET);
            return await _api.Execute(request);
        }

        public async Task<OrderBook> getOrderBook() {
            var request = new RestRequest("/returnOrderBook", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\n\t\"market\": \"ETH_ADH\",\n\t\"count\": 10\n}", ParameterType.RequestBody);
            return await _api.Execute(request);
        }
    }
}

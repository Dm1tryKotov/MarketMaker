using MMS.SupportedPlatforms.Bistox.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox.Categories
{
    public class PublicData
    {
        private BistoxRestApi _api;

        public PublicData(BistoxRestApi api) {
            this._api = api;
        }

        public async Task<OrderBookResponse> getOrderBook(string baseCurrency, string QuoteCurrency)
        {
            return await _api.Execute(new RestRequest($"api/getOrderBook/{baseCurrency}/{QuoteCurrency}", Method.GET));
        }
        public async Task<Ticker> getTicker(Models.Symbol symbol)
        {
            var res = await _api.Execute(new RestRequest($"/api/getDetail", Method.GET));

            var type = (Detail)res;
            var type2 = type.obj.Value<JObject>(symbol.BaseCurrency);
            var type3 = type2.Value<JObject>(symbol.QuoteCurrency);

            var t = new Ticker()
            {
                vol24 = type3.Value<float>("vol24")
            };
            return t;
        }

        public async Task<CandleResponse> getCandles(Models.Symbol symbol, string timeFrame)
        {
            var startDate = (Int32)DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var endDate = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return await _api.Execute(new RestRequest($"/api/getChart?baseCurrencyType={symbol.BaseCurrency}&currencyType={symbol.QuoteCurrency}&stickType={timeFrame}&start={startDate}000&end={endDate}000", Method.GET));
        }

        public async void OpenOrder(string baseCurr, string quoteCurr, string side, string price, string amount) 
        {
            var request = new RestRequest($"/api/liquidity/pushOrder", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var payload = new Payload() { 
                payload = new PayloadDetail() {
                    baseCurrencyType = baseCurr,
                    currencyType = quoteCurr,
                    price = price,
                    quantity = amount,
                    type = side
                } 
            };
            var payloadJson = JsonConvert.SerializeObject(payload);
            request.AddHeader("Authorization", "Basic dGVzdGluZ0F1dGg6dGVzdGluZ0BBdXRo");
            request.AddHeader("appname", "b2bbroker");

            request.AddParameter($"undefined", payloadJson, ParameterType.RequestBody);
            try
            {
                await _api.Execute(request);
            }
            catch 
            {
            
            }
        }

    }
}

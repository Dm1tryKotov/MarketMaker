using MMS.SupportedPlatforms.HitBtc.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Categories
{
    public class Trade
    {
        private HitBtcRestApi _api;

        public Trade(HitBtcRestApi api)
        {
            _api = api;
        }

        public async Task<Order> openOrder(
            string symbol, 
            string quantity,
            string side,
            string type,
            string price = "")
        {
            var request = new RestRequest($"/order", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", $"symbol={symbol}&quantity={quantity}&side={side}&type={type}&price={price}&undefined=", ParameterType.RequestBody);
            return await _api.Execute(request, true);
        }

        public async Task<TradesByOrder> getTradesByOrder(string id) {
            var request = new RestRequest($"history/order/{id}/trades", Method.GET);
            return await _api.Execute(request, true);
        }

        public async Task<Order> deleteOrder(string clientOrderId)
        {
            var request = new RestRequest($"order/{clientOrderId}", Method.DELETE);
            return await _api.Execute(request, true);
        }

        public async Task<Orders> deleteOrders()
        {
            var request = new RestRequest($"order", Method.DELETE);
            return await _api.Execute(request, true);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class TradesByOrder
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }

        [JsonProperty("collections")]
        public List<OneTrade> TradesList { get; set; }
    }

    public class OneTrade
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }
        [JsonProperty("quantity")]
        public float Quantity { get; set; }
        [JsonProperty("price")]
        public float Price { get; set; }
    }
}

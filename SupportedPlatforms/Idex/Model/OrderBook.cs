using Newtonsoft.Json;
using System.Collections.Generic;

namespace SupportedPlatforms.Idex.Model
{
    public class OrderBook
    {
        [JsonProperty("asks")]
        public List<OrderBookOrder> Asks { get; set; }
        [JsonProperty("bids")]
        public List<OrderBookOrder> Bids { get; set; }
    }

    public class OrderBookOrder
    {
        [JsonProperty("price")]
        public float Price { get; set; }
        [JsonProperty("amount")]
        public float Amount { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.Bistox.Model
{
    public class OrderBookResponse
    {

        [JsonProperty("result")]
        public OrderBook OrderBook { get; set; }

    }
    public class OrderBook
    {
        [JsonProperty("buys")]
        public List<OrderBookOrder> Asks { get; set; }

        [JsonProperty("sells")]
        public List<OrderBookOrder> Bids { get; set; }
    }

    public class OrderBookOrder
    {
        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("remainingQuantity")]
        public float Size { get; set; }
    }
}

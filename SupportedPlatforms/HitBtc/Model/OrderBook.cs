using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class OrderBook
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("exception")]
        public ExceptionInfo Exception { get; set; }

        [JsonProperty("ask")]
        public List<OrderBookOrder> Asks { get; set; }

        [JsonProperty("bid")]
        public List<OrderBookOrder> Bids { get; set; }
    }

    public class OrderBookOrder
    {
        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("size")]
        public float Size { get; set; }
    }
}

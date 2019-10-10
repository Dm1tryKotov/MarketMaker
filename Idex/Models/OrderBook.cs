using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Models
{
    public class OrderBook
    {
        public List<OrderBookOrder> Asks { get; private set; }
        public List<OrderBookOrder> Bids { get; private set; }

        public OrderBook(List<OrderBookOrder> asks, List<OrderBookOrder> bids) {
            Asks = asks;
            Bids = bids;
        }
    }

    public class OrderBookOrder
    {
        public float Price { get; private set; }
        public float Volume { get; private set; }

        public OrderBookOrder(float price, float volume) {
            Price = price;
            Volume = volume;
        }
    }
}

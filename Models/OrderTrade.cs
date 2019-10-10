using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Models
{
    public class OrderTrades {
        public List<OrderTrade> Trades { get; private set; }

        public OrderTrades(List<OrderTrade> trades)
        {
            Trades = trades;
        }
    }
    public class OrderTrade
    {
        /// <summary>
        /// Trade unique identifier as assigned by exchange
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Order unique identifier as assigned by exchange
        /// </summary>
        public string OrderId { get; private set; }

        /// <summary>
        /// Order unique identifier as assigned by trader
        /// </summary>
        public string ClientOrderId { get; private set; }

        public string Symbol { get; private set; }

        public string Side { get; private set; }

        public float Quantity { get; private set; }

        public float Price { get; private set; }

        public string Timestamp { get; private set; }

        public OrderTrade(
            string id = "",
            string orderId = "",
            string clientOrderId = "",
            string symbol = "",
            string side = "",
            float quantity = 0f,
            float price = 0f,
            string timestamp = ""
            )
        {
            Id = id;
            OrderId = OrderId;
            ClientOrderId = clientOrderId;
            Symbol = symbol;
            Side = side;
            Quantity = quantity;
            Price = price;
            Timestamp = timestamp;
        }
    }
}

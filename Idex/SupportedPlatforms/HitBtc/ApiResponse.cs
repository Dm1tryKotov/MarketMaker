using MMS.SupportedPlatforms.HitBtc.Model;
using System.Collections.Generic;

namespace MMS.SupportedPlatforms.HitBtc
{
    public class ApiResponse
    {
        public string Content;

        public static implicit operator Balances(ApiResponse response)
        {
            return Utilities.ConverFromJason<Balances>(response);
        }

        public static implicit operator Candles(ApiResponse response)
        {
            return Utilities.ConverFromJason<Candles>(response);
        }

        public static implicit operator OrderBook(ApiResponse response)
        {
            return Utilities.ConverFromJason<OrderBook>(response);
        }

        public static implicit operator Symbols(ApiResponse response)
        {
            return Utilities.ConverFromJason<Symbols>(response);
        }

        public static implicit operator Order(ApiResponse response)
        {
            return Utilities.ConverFromJason<Order>(response);
        }

        public static implicit operator List<Order>(ApiResponse response)
        {
            return Utilities.ConverFromJasons<Order>(response);
        }

        public static implicit operator TradesByOrder(ApiResponse response)
        {
            return Utilities.ConverFromJason<TradesByOrder>(response);
        }

        public static implicit operator Orders(ApiResponse response)
        {
            return Utilities.ConverFromJason<Orders>(response);
        }
    }
}

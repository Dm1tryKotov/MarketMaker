using MMS.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MMS.Api
{
    public interface IApiClient
    {
        void setPairName(PairName pairName);
        Symbol getSymbol();
        Balance getBalance();
        OrderBook getOrderBook();
        float get24Volume();
        List<Order> getMyOrders();
        List<Order> cancelOrders();
        Order cancelOrder(Order order);
        Order openOrder(string volume, OrderSide side, OrderType type, string price);
        OrderTrades getTradesByOrder(string id);
        Candles getCandles(int limit = 1, TimeFrame timeFrame = TimeFrame.D1);
    }
}

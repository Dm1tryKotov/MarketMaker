using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using LiquidQuoine.Net;
using LiquidQuoine.Net.Objects;
using LiquidQuoine.Net.Objects.Socket;
using MMS.Models;

namespace MMS.Api
{
    public class LiquidClient : IApiClient
    {
        private string _pairName;
        private LiquidQuoineClient _api;
        private LiquidQuoineSocketClient _socketclient;
        private LiquidQuoineClientOptions _option;
        private LiquidQuoineSocketClientOptions _socetOption;
        private Symbol _symbol;
        private int _productId;

        // token id: 1061087
        // token secret: iZOgflUeF2uRFIYvBkfUvKJq9W/UzeLxvTJT6lcp2NZrGc2JoBxV9KVqYEhU5tWtdaZKi5LsG+1LVYTUlvoAoQ==

         /*

         wmWjP6JDuc3nZDL+qtL6Rhk8T5EOPtr/30m3XvGbHYNHydodV39AVOwTcTosej9tSLVXOxMuAbSIzdiWxJdwjw==

         1063807

         */

        public LiquidClient(string apiKey, string apiSecret, PairName pairName)
        {
            _option = new LiquidQuoineClientOptions() { ApiCredentials = new ApiCredentials(apiKey, apiSecret) };
            _api = new LiquidQuoineClient(_option);
            setPairName(pairName);
        }

        private Order Convert(LiquidQuoinePlacedOrder o) //TODO: ???
        {
            return new Order(
                o.Id.ToString(),
                o.CurrencyPairCode,
                "", //liquid doesn't supply clientOrderId field
                nameof(o.Status),
                nameof(o.OrderType),
                "", //time in force
                o.CreatedAt.ToString(),
                o.UpdatedAt.ToString(),
                o.CreatedAt.ToString(), //timestamp
                (float)o.Quantity,
                (float)o.Price
                );
        }


        private OrderTrade Convert(LiquidQuoineOrderExecution o, string symbol)
        {
            return new OrderTrade(
                id: o.Id.ToString(),
                orderId: o.Id.ToString(),
                clientOrderId: o.Id.ToString(),
                symbol: symbol,
                side: nameof(o.MySide), //TODO: может быть TakerSide
                quantity: (float)o.Quantity,
                price: (float)o.Price,
                timestamp: o.CreatedAt.ToString());
        }

        public Order cancelOrder(Order order)
        {

            var response = _api.CancelOrder(long.Parse(order.Id, CultureInfo.InvariantCulture));

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка: ордербук вообще не вернулся с сервера Liquid. Возможно, не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Error.ToString());

            if (!response.Success) //TODO: хз правильно ли
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Сервер вернул неуспех.");

            return Convert(response.Data);
        }

        public List<Order> cancelOrders()
        {
            var response = _api.GetOrders(productId: _productId);
            List<Order> result = new List<Order>();
            foreach (var item in response.Data.Result)
            {
                if (item.Status == OrderStatus.Filled || item.Status == OrderStatus.Canceled) continue;
                var r2 = _api.CancelOrder(item.Id);
                result.Add(Convert(item));
            }
            return result;
        }

        public float get24Volume() //skip
        {
            var response = _api.GetProduct(_productId);

            if (response.Success)
            {
                return (float)response.Data.Volume24H;
            }
            throw new Exception($"Error: \n{response.Error.Code}\n{response.Error.Message}");
        }

        public Balance getBalance()
        {
            throw new NotImplementedException();
        }

        public Candles getCandles(int limit = 1, TimeFrame timeFrame = TimeFrame.D1) //skip
        {
            var r = _api.GetExecutions(_productId);
            // TODO выкинуть эксепшн, что торгов не было вообще и хуй там ты соберешь свечи.
            var lastProccessedMinute = 0;
            var items = r.Data.Result;
            var list = new List<Candle>();
            for (int i = 0; i < items.Count; i++)
            {
                if (i == 0)
                {
                    var candle = new Candle(
                        items[0].CreatedAt.ToString(), // TODO ОБРЕЗАТЬ СЕКУНДЫ И МИЛИСЕКУНДЫ
                        (float)items[0].Price,
                        (float)items[0].Price,
                        (float)items[0].Price,
                        (float)items[0].Price,
                        (float)items[0].Quantity
                    );
                    list.Add(candle);
                    lastProccessedMinute = items[0].CreatedAt.Minute;
                }
                else
                {
                    if (lastProccessedMinute == items[i].CreatedAt.Minute)
                    {
                        // т.е. ордер был выполнен в той же свече, что и в предыдущей
                        list.Last().SetOpen((float)items[i].Price);
                    }
                    else //если время отличается - это значит что мы рассматриваем ордер совершенный в новой свече
                    {
                        var candle = new Candle(
                            items[i].CreatedAt.ToString(), // TODO ОБРЕЗАТЬ СЕКУНДЫ И МИЛИСЕКУНДЫ
                            (float)items[i].Price,
                            (float)items[i].Price,
                            (float)items[i].Price,
                            (float)items[i].Price,
                            (float)items[i].Quantity
                        );
                        list.Add(candle);
                        lastProccessedMinute = items[i].CreatedAt.Minute;
                    }
                }

            }
            return new Candles(list);
        }

        public List<Order> getMyOrders()
        {
            throw new NotImplementedException();
        }

        public OrderBook getOrderBook()
        {
            var response = _api.GetOrderBook(_productId);
            var asks = new List<OrderBookOrder>();
            var bids = new List<OrderBookOrder>();

            foreach (var item in response.Data.BuyPriceLevels)
            {
                if (item.Amount < 100) continue;
                bids.Add(new OrderBookOrder((float)item.Price, (float)item.Amount));
            }
            foreach (var item in response.Data.SellPriceLevels)
            {
                if (item.Amount < 100) continue;
                asks.Add(new OrderBookOrder((float)item.Price, (float)item.Amount));
            }

            return new OrderBook(asks, bids);
        }

        public Symbol getSymbol()
        {
            return _symbol;
        }

        public OrderTrades getTradesByOrder(string id)
        {
            Task.Delay(1500).Wait();
            var response = _api.GetOrder(long.Parse(id));

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка: ордербук вообще не вернулся с сервера Liquid. Возможно, не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Error.ToString());

            if (!response.Success) //TODO: хз правильно ли
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Сервер вернул неуспех.");

            List<OrderTrade> orderTradeList = new List<OrderTrade>();

            foreach (var item in response.Data.Executions)
            {
                orderTradeList.Add(Convert(item, response.Data.CurrencyPairCode));
            }

            return new OrderTrades(orderTradeList);
        }

        public Order openOrder(string volume, Models.OrderSide side, Models.OrderType type, string price)
        {
            var mySide = side == Models.OrderSide.Buy 
                ? LiquidQuoine.Net.Objects.OrderSide.Buy 
                : LiquidQuoine.Net.Objects.OrderSide.Sell;

            var myType = type == Models.OrderType.Limit 
                ? LiquidQuoine.Net.Objects.OrderType.Limit 
                : LiquidQuoine.Net.Objects.OrderType.Market;

            price = price.Replace(',', '.');
            CallResult<LiquidQuoinePlacedOrder> response;

            if (price == "")
            {
                response = _api.PlaceOrder(
                    productId: _productId,
                    orderSide: mySide,
                    orderType: myType,
                    quantity: decimal.Parse(volume, CultureInfo.InvariantCulture)
                    );
            }
            else
            {
                response = _api.PlaceOrder(
                    productId: _productId,
                    orderSide: mySide,
                    orderType: myType,
                    price: decimal.Parse(price, CultureInfo.InvariantCulture),
                    quantity: decimal.Parse(volume, CultureInfo.InvariantCulture)
                    );
            }
            

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка: ордербук вообще не вернулся с сервера Liquid. Возможно, не подходит pairName.");

            if (response.Error != null)
            {
                if (response.Error.Message.Contains("nonce"))
                {
                    throw new Exception("nonce");
                }
                if (response.Error.Message.Contains("order"))
                {
                    throw new Exception("");
                }
                throw new Exception(response.Error.ToString());
            }

            if (!response.Success) //TODO: хз правильно ли
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Сервер вернул неуспех.");

            return new Order(
                response.Data.Id.ToString(),
                response.Data.CurrencyPairCode,
                "", //liquid doesn't supply clientOrderId field
                nameof(response.Data.Status),
                nameof(response.Data.OrderType),
                "", //time in force
                response.Data.CreatedAt.ToString(),
                response.Data.UpdatedAt.ToString(),
                response.Data.CreatedAt.ToString(), //timestamp
                (float)response.Data.Quantity,
                (float)response.Data.Price
                );
        }

        public void setPairName(PairName pairName)
        {
            var result = _api.GetAllProducts();
            switch (pairName)
            {
                case PairName.ETHADH:
                    foreach (var item in result.Data)
                    {
                        if (item.CurrencyPairCode == "ADHETH")
                        {
                            _symbol = new Symbol(
                                  item.QuotedCurrency,
                                  item.BaseCurrency,
                                  // TODO количество знаков после запятой пересчитать
                                  0.00000001f,
                                  100
                                  );
                            _productId = item.Id;
                            _pairName = item.CurrencyPairCode;
                        }
                    }
                    break;
                case PairName.BTCADH:
                    foreach (var item in result.Data)
                    {
                        if (item.CurrencyPairCode == "ADHBTC")
                        {
                            _symbol = new Symbol(
                                  item.QuotedCurrency,
                                  item.BaseCurrency,
                                  // TODO количество знаков после запятой пересчитать
                                  0.00000001f,
                                  100
                                  );
                            _productId = item.Id;
                            _pairName = item.CurrencyPairCode;
                        }
                    }
                    break;
            }
        }
    }
}

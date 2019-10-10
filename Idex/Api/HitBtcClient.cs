using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMS.Models;
using MMS.SupportedPlatforms.HitBtc;

namespace MMS.Api
{
    public class HitBtcClient : IApiClient
    {
        private string _pairName;
        private HitBtcRestApi _api;
        private Symbol _symbol;

        public HitBtcClient(string apiKey, string apiSecret, PairName pairName)
        {
            _api = new HitBtcRestApi();
            _api.Auth(apiKey, apiSecret);
            setPairName(pairName);
        }

        public Order cancelOrder(Order order)
        {
            var response = _api.Trade.deleteOrder(order.ClientOrderId).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Ордербук вообще не вернулся с сервера HitBtc. Возможно не подходит pairName.");
          
            if (response.Error != null && response.Error.Code.Equals("20002"))
                return new Order();

            if (response.Error != null && response.Error.Code.Equals("20002"))
                throw new Exception(response.Exception.ToString());

            if (response.Exception != null)
                throw new Exception(response.Error.ToString());

            return new Order(
                response.Id,
                response.Symbol,
                response.ClientOrderId,
                response.Status,
                response.Type,
                response.TimeInForce,
                response.CreatedAt,
                response.UpdatedAt,
                response.timestamp,
                response.Quantity,
                response.Price
                );
        }

        public List<Order> cancelOrders()
        {
            var response = _api.Trade.deleteOrders().Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Ордербук вообще не вернулся с сервера HitBtc. Возможно не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Error.ToString());

            if (response.Exception != null)
                throw new Exception(response.Exception.ToString());

            var reslult = new List<Order>();

            foreach (var item in response.OrderList) {
                reslult.Add(
                    new Order(
                        item.Id,
                        item.Symbol,
                        item.ClientOrderId,
                        item.Status,
                        item.Type,
                        item.TimeInForce,
                        item.CreatedAt,
                        item.UpdatedAt,
                        item.timestamp,
                        item.Quantity,
                        item.Price
                    )
                );
            }

            return reslult;
        }

        public float get24Volume()
        {
            var response = _api.PublicData.getCandles(_pairName, 1, TimeFrame.D1.ToString()).Result;
            
            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Ордербук вообще не вернулся с сервера HitBtc. Возможно не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Exception.ToString());

            if (response.Exception != null)
                throw new Exception(response.Error.ToString());
            
            return response.CandlesList[0].vol;
        }

        public Balance getBalance()
        {
            var response = _api.Account.GetBalance().Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Ордербук вообще не вернулся с сервера HitBtc. Возможно не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Exception.ToString());

            if (response.Exception != null)
                throw new Exception(response.Error.ToString());

            if(_symbol == null)
                throw new Exception("инструмент не найден, перед запросом баланса - необходимо получить данные по инструменту");

            BalanceItem baseBalance = null;
            BalanceItem quoteBalance = null;

            foreach (var item in response.BalanceList)
            {
                if (item.Currency == _symbol.BaseCurrency)
                {
                    baseBalance = new BalanceItem(_symbol.BaseCurrency, item.Available);
                }
                if (item.Currency == _symbol.QuoteCurrency)
                {
                    quoteBalance = new BalanceItem(_symbol.QuoteCurrency, item.Available);
                }
            }

            if(baseBalance == null)
                baseBalance = new BalanceItem(_symbol.BaseCurrency, 0);
            if (quoteBalance == null)
                quoteBalance = new BalanceItem(_symbol.QuoteCurrency, 0);

            return new Balance(baseBalance, quoteBalance);
        }

        public Candles getCandles(int limit = 1, TimeFrame timeFrame = TimeFrame.D1)
        {
            var response = _api.PublicData.getCandles(_pairName, limit, timeFrame.ToString()).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Ордербук вообще не вернулся с сервера HitBtc. Возможно не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Exception.ToString());

            if (response.Exception != null)
                throw new Exception(response.Error.ToString());

            var candles = new List<Candle>();

            foreach (var item in response.CandlesList)
            {
                candles.Add(new Candle(
                    item.time,
                    item.open,
                    item.close,
                    item.high,
                    item.low,
                    item.vol
                    ));
            }

            return new Candles(candles);
        }

        public List<Order> getMyOrders()
        {
            var response = _api.Account.GetOrders(_pairName).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Ордербук вообще не вернулся с сервера HitBtc. Возможно не подходит pairName.");

            if (response.Error != null)
                throw new Exception(response.Error.ToString());

            if (response.Exception != null)
                throw new Exception(response.Exception.ToString());

            List<Order> result = new List<Order>();
            foreach(var item in response.OrderList) {
                result.Add(new Order(
                    item.Id,
                    item.Symbol,
                    item.ClientOrderId,
                    item.Status,
                    item.Type,
                    item.TimeInForce,
                    item.CreatedAt,
                    item.UpdatedAt,
                    item.timestamp,
                    item.Quantity,
                    item.Price,
                    item.Side
                    ));
            }
            return result;
        }

        public OrderBook getOrderBook()
        {
            var response = _api.PublicData.getOrderBook(_pairName).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Сервер ничего не вернул. Следует обратить внимаение на URL + параметры.");

            if (response.Error != null)
                throw new Exception(response.Exception.ToString());
            
            if (response.Exception != null)
                throw new Exception(response.Error.ToString());

            var asks = new List<OrderBookOrder>();
            var bids = new List<OrderBookOrder>();

            foreach (var item in response.Asks)
            {
                asks.Add(new OrderBookOrder(item.Price, item.Size));
            }
            foreach (var item in response.Bids)
            {
                bids.Add(new OrderBookOrder(item.Price, item.Size));
            }

            return new OrderBook(asks, bids);
        }

        public Symbol getSymbol()
        {
            var response = _api.PublicData.getSymbols().Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Сервер ничего не вернул. Следует обратить внимаение на URL + параметры.");

            if (response.Error != null)
                throw new Exception(response.Exception.ToString());

            if (response.Exception != null)
                throw new Exception(response.Error.ToString());

            foreach (var item in response.SymbolsList) {
                if (item.Id == _pairName) {
                    _symbol = new Symbol(item.QuoteCurrency, item.BaseCurrency, item.TickSize, item.QuantityIncrement);
                    return _symbol;
                }
            }

            throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. на платформе HitBtc не найден инструмент {_pairName}.");
        }

        public OrderTrades getTradesByOrder(string id)
        {
            Task.Delay(1500).Wait();
            var response = _api.Trade.getTradesByOrder(id).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Сервер ничего не вернул. Следует обратить внимаение на URL + параметры.");

            if (response.Error != null)
            {
                throw new Exception(response.Error.ToString());
            }

            if (response.Exception != null)
            {
                throw new Exception(response.Exception.ToString());
            }

            if (response.TradesList == null || response.TradesList.Count == 0)
            {
                return null;
            }
            var tradeList = new List<OrderTrade>();
            foreach (var item in response.TradesList) {
                tradeList.Add(new OrderTrade(item.Id,
                    item.OrderId,
                    item.ClientOrderId,
                    price: item.Price,
                    quantity: item.Quantity));
            }
            return new OrderTrades(tradeList);
        }

        public Order openOrder(string volume, OrderSide side, OrderType type, string price)
        {
            var mySide = side.ToString().ToLower();
            var myType = type.ToString().ToLower();
            price = price.Replace(',', '.');

            var response = _api.Trade.openOrder(_pairName, volume, mySide, myType, price).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Сервер ничего не вернул. Следует обратить внимаение на URL + параметры.");

            if (response.Error != null)
            {
                throw new Exception(response.Error.ToString());
            }

            if (response.Exception != null)
            {
                throw new Exception(response.Exception.ToString());
            }

            return new Order(
                response.Id,
                response.Symbol,
                response.ClientOrderId,
                response.Status,
                response.Type,
                response.TimeInForce,
                response.CreatedAt,
                response.UpdatedAt,
                response.timestamp,
                response.Quantity,
                response.Price,
                response.Side
                );
        }

        public void setPairName(PairName pairName)
        {
            switch (pairName)
            {
                case PairName.ETHADH:
                    _pairName = "ADHETH";
                    break;
                case PairName.BTCADH:
                    _pairName = "ADHBTC";
                    break;
                case PairName.ETHLRC:
                    _pairName = "LRCETH";
                    break;
                case PairName.BTCLRC:
                    _pairName = "LRCBTC";
                    break;
            }
        }
    }
}
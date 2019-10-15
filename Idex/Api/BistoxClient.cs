using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMS.Models;
using MMS.SupportedPlatforms.Bistox;

namespace MMS.Api
{
    public class BistoxClient : IApiClient
    {
        private string _pairName;
        private BistoxRestApi _api;
        private Symbol _symbol;

        public BistoxClient(string apiKey, string apiSecret, PairName pairName)
        {
            _api = new BistoxRestApi();
            setPairName(pairName);
        }
        public Order cancelOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public List<Order> cancelOrders()
        {
            throw new NotImplementedException();
        }

        public float get24Volume()
        {
            var response = _api.PublicData.getTicker(_symbol).Result;
            return response.vol24;
        }

        public Balance getBalance()
        {
            throw new NotImplementedException();
        }

        public Candles getCandles(int limit = 1, TimeFrame timeFrame = TimeFrame.D1)
        {
            string newTimeFrame = "";
            switch (timeFrame)
            {
                case TimeFrame.M1:
                    newTimeFrame = "ONE_MINUTE";
                    break;
                case TimeFrame.M3:
                    break;
                case TimeFrame.M5:
                    newTimeFrame = "FIVE_MINUTE";
                    break;
                case TimeFrame.M15:
                    newTimeFrame = "FIFTEEN_MINUTE";
                    break;
                case TimeFrame.M30:
                    newTimeFrame = "THIRTY_MINUTE";
                    break;
                case TimeFrame.H1:
                    newTimeFrame = "ONE_HOUR";
                    break;
                case TimeFrame.H4:
                    newTimeFrame = "FOUR_HOUR";
                    break;
                case TimeFrame.D1:
                    newTimeFrame = "ONE_DAY";
                    break;
                case TimeFrame.D7:
                    newTimeFrame = "ONE_WEEK";
                    break;
            }

            var response = _api.PublicData.getCandles(_symbol, newTimeFrame).Result;
            var candleList = new List<Candle>();

            foreach(var item in response.candles)
            {
                candleList.Add(new Candle(
                    new DateTime(item.time).ToString(),
                    item.open,
                    item.close,
                    item.high,
                    item.low,
                    item.volume));
            }

            return new Candles(candleList);
        }

        public List<Order> getMyOrders()
        {
            throw new NotImplementedException();
        }

        public OrderBook getOrderBook()
        {
            var response = _api.PublicData.getOrderBook(_symbol.BaseCurrency, _symbol.QuoteCurrency).Result;

            if (response == null)
                throw new Exception($"[{DateTime.UtcNow.TimeOfDay}] Критическая ошибка. Сервер ничего не вернул. Следует обратить внимаение на URL + параметры.");
            var orderBook = response.OrderBook;

            var asks = new List<OrderBookOrder>();
            var bids = new List<OrderBookOrder>();

            foreach (var item in orderBook.Bids)
            {
                asks.Add(new OrderBookOrder(item.Price, item.Size));
            }
            foreach (var item in orderBook.Asks)
            {
                bids.Add(new OrderBookOrder(item.Price, item.Size));
            }

            return new OrderBook(asks, bids);
        }

        public Symbol getSymbol()
        {
            return _symbol;
        }

        public OrderTrades getTradesByOrder(string id)
        {
            throw new NotImplementedException();
        }

        public Order openOrder(string volume, OrderSide side, OrderType type, string price)
        {
            price = price.Replace(',', '.');
            var s = side == OrderSide.Buy ? "buy" : "sell";

            if (type == OrderType.Limit)
            {
                _api.PublicData.OpenOrder(_symbol.BaseCurrency, _symbol.QuoteCurrency, s, price, volume);
            }
            else
            {
                var a = getOrderBook();
                if(side == OrderSide.Buy)
                {
                    price = a.Asks[0].Price.ToString();
                }
                else
                {
                    price = a.Bids[0].Price.ToString();
                }

                price = price.Replace(',', '.');
                _api.PublicData.OpenOrder(_symbol.BaseCurrency, _symbol.QuoteCurrency, s, price, volume);
            }
            return new Order();
        }

        public void setPairName(PairName pairName)
        {
            switch (pairName)
            {
                case PairName.ETHXRP:
                    _pairName = "ethxrp";
                    _symbol = new Symbol(
                        "xrp",
                        "eth",
                        0.00000001f,
                        1
                        );
                    break;
                case PairName.ETHXLM:
                    _pairName = "ethxlm";
                    _symbol = new Symbol(
                        "xlm",
                        "eth",
                        0.00000001f,
                        1
                        );
                    break;
                case PairName.ETHADH:
                    break;
                case PairName.BTCADH:
                    break;
                case PairName.ETHLRC:
                    break;
                case PairName.BTCLRC:
                    break;
                case PairName.ETHBCH:
                    _pairName = "ethbch";
                    _symbol = new Symbol(
                        "bch",
                        "eth",
                        0.00000001f,
                        0.0001f
                        );
                    break;
                case PairName.ETHLTC:
                    _pairName = "ethltc";
                    _symbol = new Symbol(
                        "ltc",
                        "eth",
                        0.00000001f,
                        0.0001f
                        );
                    break;
                case PairName.ETHXEM:
                    _pairName = "ethxem";
                    _symbol = new Symbol(
                        "xem",
                        "eth",
                        0.00000001f,
                        0.0001f
                    );
                    break;
                case PairName.ETHUSDT:
                    _pairName = "ethusdt";
                    _symbol = new Symbol(
                        "usdt",
                        "eth",
                        0.00000001f,
                        0.0001f
                    );
                    break;
                case PairName.BTCBCH:
                    _pairName = "btcbch";
                    _symbol = new Symbol(
                        "bch",
                        "btc",
                        0.00000001f,
                        0.0001f
                        );
                    break;
                case PairName.BTCETH:
                    _pairName = "btcbch";
                    _symbol = new Symbol(
                        "bch",
                        "btc",
                        0.00000001f,
                        0.0001f
                        );
                    break;
                case PairName.BTCXRP:
                    _pairName = "btcxrp";
                    _symbol = new Symbol(
                        "xrp",
                        "btc",
                        0.00000001f,
                        1f
                        );
                    break;
                case PairName.BTCXLM:
                    _pairName = "btcxlm";
                    _symbol = new Symbol(
                        "xlm",
                        "btc",
                        0.00000001f,
                        1f
                        );
                    break;
                case PairName.BTCLTC:
                    _pairName = "btcltc";
                    _symbol = new Symbol(
                        "ltc",
                        "btc",
                        0.00000001f,
                        0.001f
                        );
                    break;
                case PairName.BTCXEM:
                    _pairName = "btcxem";
                    _symbol = new Symbol(
                        "xem",
                        "btc",
                        0.00000001f,
                        1f
                        );
                    break;
            }
        }
    }
}
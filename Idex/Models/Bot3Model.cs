using MMS.Api;
using MMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMS.Models
{
    public class Bot3Model : INotifyPropertyChanged
    {
        private string _log;
        public string Log
        {
            get { return _log; }
            private set
            {
                this.MutateVerbose(ref _log, value, RaisePropertyChanged());
            }
        }

        public float MyTotalVolume
        {
            get { return _myTotalVolume; }
            set { this.MutateVerbose(ref _myTotalVolume, value, RaisePropertyChanged()); }
        }
        private float _myTotalVolume;
        public float MyOrdersCount
        {
            get { return _myOrdersCount; }
            set { this.MutateVerbose(ref _myOrdersCount, value, RaisePropertyChanged()); }
        }
        private float _myOrdersCount;
        public float MyBidVolume
        {
            get { return _myBidVolume; }
            set { this.MutateVerbose(ref _myBidVolume, value, RaisePropertyChanged()); }
        }
        private float _myBidVolume;
        public float MyAskVolume
        {
            get { return _myAskVolume; }
            set { this.MutateVerbose(ref _myAskVolume, value, RaisePropertyChanged()); }
        }
        private float _myAskVolume;
        public float MyAsksCount
        {
            get { return _myAsksCount; }
            set { this.MutateVerbose(ref _myAsksCount, value, RaisePropertyChanged()); }
        }
        private float _myAsksCount;
        public float MyBidsCount
        {
            get { return _myBidsCount; }
            set { this.MutateVerbose(ref _myBidsCount, value, RaisePropertyChanged()); }
        }
        private float _myBidsCount;

        private bool _botStarted;
        public bool BotStarted
        {
            get { return _botStarted; }
            set
            {
                _botStarted = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _ident;
        private int _depth;
        private int _orderSpace = 1;
        private int _range;
        private int _directionStrong;
        private float _sellQuantityLeft;
        private float _buyQuantityLeft;
        private float _volume;

        private IApiClient _client1;
        private Direction _direction;
        private Symbol _symbol;
        private OrderBook _orderBook;
        private List<Order> _openedOrders;
        private List<Order> _myNewOrders;

        public Bot3Model()
        {
        }

        private void cancelOrders()
        {
            Log += $"\n[{DateTime.Now.TimeOfDay}] начинаю отменять ордера.";
            try
            {
                foreach (var item in _myNewOrders)
                {
                    _client1.cancelOrder(item);
                }
                _myNewOrders.Clear();
            }
            catch (Exception e)
            {
                Log += $"\n[{DateTime.Now.TimeOfDay}] не удалось отменить ордера. Ошибка: \n{e.Message}";
            }
            Log += $"\n[{DateTime.Now.TimeOfDay}] все ордера сняты";
        }

        public void stopWork()
        {
            Log += $"\n[{DateTime.Now.TimeOfDay}] Отменяем ордера";
            cancelOrders();
            Log += $"\n[{DateTime.Now.TimeOfDay}] Бот остановлен";
        }

        public void startWork(
            IApiClient client1,
            Direction direction,
            int ident,
            int depth,
            float volume,
            int orderSpace,
            int directionStrong,
            int range,
            CancellationToken ct
            )
        {
            _myNewOrders = new List<Order>();
            _client1 = client1;
            _direction = direction;
            _ident = ident;
            _depth = depth;
            _volume = volume;
            _orderSpace = orderSpace;
            _directionStrong = directionStrong;
            _range = range;
            var r = new Random();
            while (true)
            {
                try
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            _orderBook = _client1.getOrderBook();
                            return;
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] ордербук не загружен. Ошибка: \n{e.Message}";
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            _botStarted = true;
                            BotStarted = false;
                            throw new Exception(e.ToString());
                        }
                    }, ct).Wait(ct);

                    Task.Run(() =>
                    {
                        try
                        {
                            if (_symbol == null)
                                _symbol = _client1.getSymbol();
                            return;
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] инструмент не загружен. Ошибка: \n{e.Message}";
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            _botStarted = true;
                            BotStarted = false;
                            throw new Exception(e.ToString());
                        }
                    }, ct).Wait(ct);

                    Task.Run(() =>
                    {
                        try
                        {
                            _openedOrders = _client1.getMyOrders();
                            return;
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] информация об открытых ордерах не загружена. Ошибка: \n{e.Message}";
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            _botStarted = true;
                            BotStarted = false;
                            throw new Exception(e.ToString());
                        }
                    }, ct).Wait(ct);
                }
                catch (OperationCanceledException)
                {
                    Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание отмена операции.";
                    break;
                }
                catch (Exception)
                {
                    break;
                }

                if (direction == Direction.Flat)
                {
                    _sellQuantityLeft = _volume / 2;
                    _buyQuantityLeft = _volume / 2;
                }
                else
                {
                    _sellQuantityLeft = direction == Direction.Up
                        ? volume - volume / 100 * _directionStrong
                        : volume / 100 * _directionStrong;

                    _buyQuantityLeft = direction == Direction.Up
                        ? volume / 100 * _directionStrong
                        : volume - volume / 100 * _directionStrong;
                }

                var askOrdersList = new List<Tuple<float, float>>();
                var bidOrdersList = new List<Tuple<float, float>>();
                
                var myAskOrders = new List<Order>();
                var myBidOrders = new List<Order>();

                foreach (var item in _openedOrders)
                {
                    foreach (var item2 in _myNewOrders)
                    {
                        if (item.Id == item2.Id)
                        {
                            if (item.Side.ToLower().Equals("sell")) myAskOrders.Add(item);
                            if (item.Side.ToLower().Equals("buy")) myBidOrders.Add(item);
                        }
                    }
                }

                if (myAskOrders.Count > 0 && myAskOrders.Count < _depth)
                {
                    Task.Run(() => {
                        _client1.cancelOrders();
                        myAskOrders.Clear();
                        myBidOrders.Clear();
                    }).Wait();
                }
                if (myBidOrders.Count > 0 && myBidOrders.Count < _depth)
                {
                    Task.Run(() => {
                        _client1.cancelOrders();
                        myAskOrders.Clear();
                        myBidOrders.Clear();
                    }).Wait();
                }

                if (_sellQuantityLeft >= _symbol.QuantityIncrement)
                {
                    askOrdersList.Clear();
                    var minusCount = myAskOrders.Count;
                    var lastPrice = 0f;

                    var valueList = new List<int>();
                    if (_depth - minusCount != 0)
                    {
                        valueList = getPyramid_valueList((int)_sellQuantityLeft, _depth - minusCount, _symbol, r);
                    }

                    for (var i = 0; i < _depth - minusCount; i++)
                    {
                        float price;
                        if (myAskOrders.Count > 0)
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (lastPrice == 0)
                            {
                                price = myAskOrders.Last().Price + _symbol.TickSize;
                                lastPrice = price;
                            }
                            else
                            {
                                price = lastPrice + _symbol.TickSize * _orderSpace;
                                lastPrice = price;
                            }
                        }
                        else
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (lastPrice == 0)
                            {
                                price = _orderBook.Asks[0].Price + _symbol.TickSize * _ident;
                                lastPrice = price;
                            }
                            else
                            {
                                price = lastPrice + _symbol.TickSize * _orderSpace;
                                lastPrice = price;
                            }
                        }

                        var item = new Tuple<float, float>(valueList[i], price);
                        if (item.Item1 > 0) askOrdersList.Add(item);
                    }
                }

                if (_buyQuantityLeft > _symbol.QuantityIncrement)
                {
                    bidOrdersList.Clear();
                    var minusCount = myBidOrders.Count;
                    var lastPrice = 0f;

                    var valueList = new List<int>();
                    if (_depth - minusCount != 0)
                    {
                        valueList = getPyramid_valueList((int)_buyQuantityLeft, _depth - minusCount, _symbol, r);
                    }

                    for (var i = 0; i < _depth - minusCount; i++)
                    {
                        float price;
                        if (myBidOrders.Count > 0)
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (lastPrice == 0)
                            {
                                price = myBidOrders.Last().Price - _symbol.TickSize;
                                lastPrice = price;
                            }
                            else
                            {
                                price = lastPrice - _symbol.TickSize * _orderSpace;
                                lastPrice = price;
                            }
                        }
                        else
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (lastPrice == 0)
                            {
                                price = _orderBook.Bids[0].Price - _symbol.TickSize * _ident;
                                lastPrice = price;
                            }
                            else
                            {
                                price = lastPrice - _symbol.TickSize * _orderSpace;
                                lastPrice = price;
                            }
                        }

                        var item = new Tuple<float, float>(valueList[i], price);
                        if (item.Item1 > 0) bidOrdersList.Add(item);
                    }
                }

                var side = OrderSide.Sell;
                var type = OrderType.Limit;
                try
                {
                    foreach (var item in askOrdersList)
                    {
                        var price = item.Item2.ToString("0.00000000000");
                        price = price.Replace(',', '.');
                        var amount = item.Item1.ToString(CultureInfo.CurrentCulture);
                        try
                        {
                            Task.Run(() =>
                            {
                                var order = client1.openOrder(amount, side, type, price);
                                Log += $"\n[{DateTime.Now.TimeOfDay}] открываем {type} ордер, объемом {amount}, в сторону {side}, по цене {price: 0.0000000000}";
                                _myNewOrders.Add(order);
                            }, ct).Wait(ct);
                        }
                        catch (OperationCanceledException)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            throw new Exception();
                        }
                    }

                    foreach (var item in bidOrdersList)
                    {
                        var price = item.Item2.ToString("0.00000000000");
                        price = price.Replace(',', '.');
                        side = OrderSide.Buy;
                        var amount = item.Item1.ToString(CultureInfo.CurrentCulture);
                        try
                        {
                            Task.Run(() =>
                            {
                                var order = client1.openOrder(amount, side, type, price);
                                Log += $"\n[{DateTime.Now.TimeOfDay}] открываем {type} ордер, объемом {amount}, в сторону {side}, по цене {price: 0.0000000000}";
                                _myNewOrders.Add(order);
                            }, ct).Wait(ct);
                        }
                        catch (OperationCanceledException)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            throw new Exception();
                        }
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private static List<int> getPyramid_valueList(int totalAmount, int depth, Symbol symbol,
            Random rand)
        {
            var result = new List<int>();
            var total = totalAmount;
            var r = rand.Next(51, 80) / 100.0f;
            var pctStep = (1 - r) / depth;
            for (var i = 0; i < depth; i++)
            {
                float value;
                if (i == depth - 1)
                {
                    value = total;
                    value -= value % (int)symbol.QuantityIncrement;
                    result.Add((int)value);
                }
                else
                {
                    // ReSharper disable once PossibleLossOfFraction
                    value = total / (depth - i) * r;
                    r += pctStep;
                    value -= value % symbol.QuantityIncrement;
                    total -= (int)value;
                    result.Add((int)value);
                }
            }

            return result;
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}

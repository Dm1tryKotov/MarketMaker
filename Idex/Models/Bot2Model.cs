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
    public class Bot2Model : INotifyPropertyChanged
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
        public int MyAsksCount
        {
            get { return _myAsksCount; }
            set { this.MutateVerbose(ref _myAsksCount, value, RaisePropertyChanged()); }
        }
        private int _myAsksCount;
        public int MyBidsCount
        {
            get { return _myBidsCount; }
            set { this.MutateVerbose(ref _myBidsCount, value, RaisePropertyChanged()); }
        }
        private int _myBidsCount;

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

        public Bot2Model()
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
            _orderSpace = orderSpace == 0 ? 1 : orderSpace;
            _directionStrong = directionStrong;
            _range = range;
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
                            Log += $"\n[{DateTime.Now.TimeOfDay}] ордербук не загружен.";
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
                            Log += $"\n[{DateTime.Now.TimeOfDay}] информация об инструменте не получена.";
                            Log += $"\n[{DateTime.Now.TimeOfDay}] {e.ToString()}";
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
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Во время загрузки открытых ордеров произошла ошибка.";
                            Log += $"\n[{DateTime.Now.TimeOfDay}] {e.ToString()}";
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
                

                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var myAskOrders = new List<Order>();
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var myBidOrders = new List<Order>();

                foreach (var item in _openedOrders)
                {
                    foreach (var item2 in _myNewOrders) {
                        if (item.Id == item2.Id) {
                            if (item.Side.ToLower().Equals("sell")) myAskOrders.Add(item);
                            if (item.Side.ToLower().Equals("buy")) myBidOrders.Add(item);
                        }
                    }
                }
                
                for (int i = 0; i < myAskOrders.Count; i++)
                {
                    if (myAskOrders[i].Price > _orderBook.Asks[0].Price + (_ident + i * _orderSpace - _range - 1) * _symbol.TickSize
                        && myAskOrders[i].Price < _orderBook.Asks[0].Price + (_ident + i * _orderSpace + _range + 1) * _symbol.TickSize) continue;

                    Task.Run(()=> {
                        _client1.cancelOrder(myAskOrders[i]);
                    }).Wait();

                    for (int j = 0; j < _myNewOrders.Count; j++)
                    {
                        if (_myNewOrders[i].Id.Equals(myAskOrders[i].Id))
                            _myNewOrders.RemoveAt(j);
                    }
                    myAskOrders.RemoveAt(i);
                }

                for (int i = 0; i < myBidOrders.Count; i++)
                {
                    if (myBidOrders[i].Price > _orderBook.Bids[0].Price - (_ident + i * _orderSpace + _range + 1) * _symbol.TickSize
                        && myBidOrders[i].Price < _orderBook.Bids[0].Price - (_ident + i * _orderSpace - _range - 1) * _symbol.TickSize) continue;

                    Task.Run(() => {
                        _client1.cancelOrder(myBidOrders[i]);
                    }).Wait();
                    for (int j = 0; j < _myNewOrders.Count; j++)
                    {
                        if (_myNewOrders[i].Id.Equals(myBidOrders[i].Id))
                            _myNewOrders.RemoveAt(j);
                    }
                    myBidOrders.RemoveAt(i);
                }

                var askValue = 0f;
                var bidValue = 0f;
                if (myAskOrders.Count > 0)
                {
                    foreach (var item in myAskOrders)
                    {
                        _sellQuantityLeft -= item.Quantity;
                        askValue += item.Quantity;
                    }
                }

                if (myBidOrders.Count > 0)
                {
                    foreach (var item in myBidOrders)
                    {
                        _buyQuantityLeft -= item.Quantity;
                        bidValue += item.Quantity;
                    }
                }
                MyAsksCount = myAskOrders.Count;
                MyAskVolume = 0;
                foreach (var item in myAskOrders)
                {
                    MyAskVolume += item.Quantity;
                }

                MyBidVolume = 0;
                MyBidsCount = myBidOrders.Count;
                foreach (var item in myBidOrders)
                {
                    MyBidVolume += item.Quantity;
                }
                MyOrdersCount = MyAsksCount + MyBidsCount;
                MyTotalVolume = MyAskVolume + MyBidVolume;


                if (_sellQuantityLeft >= _symbol.QuantityIncrement)
                {                    askOrdersList.Clear();
                    var minusCount = myAskOrders.Count;
                    var lastPrice = 0f;

                    var valueList = new List<int>();
                    if (_depth - minusCount != 0)
                    {
                        valueList = getRandom_valueList((int)_sellQuantityLeft, _depth - minusCount, _symbol);
                        if (valueList == null || valueList.Count != _depth - minusCount)
                        {
                            continue;
                        }
                    }

                    for (var i = 0; i < _depth - minusCount; i++)
                    {
                        float price;
                        if (myAskOrders.Count > 0)
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (lastPrice == 0f)
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

                if (_buyQuantityLeft >= _symbol.QuantityIncrement)
                {
                    bidOrdersList.Clear();
                    var minusCount = myBidOrders.Count;
                    var lastPrice = 0f;

                    var valueList = new List<int>();
                    if (_depth - minusCount != 0)
                    {
                        valueList = getRandom_valueList((int)_buyQuantityLeft, _depth - minusCount, _symbol);
                        if (valueList == null || valueList.Count != _depth - minusCount)
                        {
                            continue;
                        }
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
                Task.Delay(1000).Wait();
            }
        }

        private static List<int> getRandom_valueList(int totalAmount, int depth, Symbol symbol)
        {
            var result = new List<int>();
            var onePart = totalAmount / depth;

            if (onePart >= (int)symbol.QuantityIncrement)
            {
                onePart = onePart - onePart % (int)symbol.QuantityIncrement;
            }
            else
            {
                return null;
            }

            var b = totalAmount - depth * onePart;

            var r = new Random();
            for (var i = 0; i < depth; i++)
            {
                var pct = r.Next(20, 100) / 100.0f;
                var myNumber = (int)(onePart * pct);
                if (myNumber > (int)symbol.QuantityIncrement)
                {
                    b += myNumber % (int)symbol.QuantityIncrement;
                    myNumber -= myNumber % (int)symbol.QuantityIncrement;

                    b += onePart - (int)(onePart * pct);
                }
                else
                {
                    myNumber = (int)symbol.QuantityIncrement;
                    b += onePart - myNumber;
                }

                result.Add(myNumber);
            }

            if (b / depth >= (int)symbol.QuantityIncrement)
            {
                b /= depth;

                var s = b - b % (int)symbol.QuantityIncrement;

                b -= s;
                b *= depth;

                for (var i = 0; i < depth; i++)
                {
                    result[i] += s;
                }
            }

            var wasSelected = new List<int>();
            for (var i = 0; i < b / (int)symbol.QuantityIncrement; i++)
            {
                var index = r.Next(0, result.Count);
                while (wasSelected.Contains(index))
                {
                    index = r.Next(0, result.Count);
                }

                result[r.Next(0, result.Count)] += (int)symbol.QuantityIncrement;
                wasSelected.Add(index);
            }

            return result;
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}

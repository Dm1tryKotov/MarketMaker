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
    public class Bot4Model : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        private int _ident;
        private int _range;
        private float _volume;

        private IApiClient _client;
        private Symbol _symbol;
        private OrderBook _orderBook;
        private List<Order> _openedOrders;
        private List<Order> _myNewOrders;

        public Bot4Model()
        {
        }

        private void cancelOrders()
        {
            Log += $"\n[{DateTime.Now.TimeOfDay}] начинаю отменять ордера.";
            try
            {
                foreach (var item in _myNewOrders)
                {
                    _client.cancelOrder(item);
                }
                _myNewOrders.Clear();
                MyTotalVolume = 0;
                MyOrdersCount = 0;
                MyBidVolume = 0;
                MyAskVolume = 0;
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
            IApiClient client,
            int ident,
            int depth,
            float volume,
            int orderSpace,
            int directionStrong,
            int range,
            CancellationToken ct
            )
        {
            _openedOrders = new List<Order>();
            _myNewOrders = new List<Order>();
            _client = client;
            _ident = ident;
            _volume = volume;
            _range = range;
            Log = string.Empty;
            var r = new Random();
            while (true)
            {
                try
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            _orderBook = _client.getOrderBook();
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
                            {
                                _symbol = _client.getSymbol();
                                Log += $"\n[{DateTime.Now.TimeOfDay}] информация об инструменте получена.";
                            }
                            return;
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] информация об инструменте не получена.";
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
                            _openedOrders = _client.getMyOrders();
                            return;
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] список открытых ордеров не получен";
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

                try
                {
                    for (int i = 0; i < myAskOrders.Count; i++)
                    {
                        if (!(myAskOrders[i].Price <= _orderBook.Asks[0].Price + (_ident - _range - 1) * _symbol.TickSize)
                            && !(myAskOrders[i].Price >= _orderBook.Asks[0].Price + (_ident + _range + 1) * _symbol.TickSize))
                        {
                            Task.Delay(1000).Wait();
                            continue;
                        }

                        Log += $"\n[{DateTime.Now.TimeOfDay}] Наш аск превысил допустимое отклонение";

                        try
                        {
                            Task.Run(() => {
                                Log += $"\n[{DateTime.Now.TimeOfDay}] Снимаем ask ордер";
                                _client.cancelOrder(myAskOrders[i]);
                            }).Wait();
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Ошибка при снятии ордера!\n{e.InnerException}";
                            throw new Exception();
                        }

                        for (int j = 0; j < _myNewOrders.Count; j++)
                        {
                            if (_myNewOrders[j].Id.Equals(myAskOrders[i].Id))
                            {
                                _myNewOrders.RemoveAt(j);
                            }
                        }
                        myAskOrders.Clear();
                        continue;
                    }
                    for (int i = 0; i < myBidOrders.Count; i++)
                    {
                        if (!(myBidOrders[i].Price >= _orderBook.Bids[0].Price - (_ident - _range - 1) * _symbol.TickSize)
                            && !(myBidOrders[i].Price <= _orderBook.Bids[0].Price - (_ident + _range + 1) * _symbol.TickSize))
                        {
                            Task.Delay(1000).Wait();
                            continue;
                        }

                        Log += $"\n[{DateTime.Now.TimeOfDay}] Наш бид превысил допустимое отклонение";

                        try
                        {
                            Task.Run(() => {
                                Log += $"\n[{DateTime.Now.TimeOfDay}] Снимаем bid ордер";
                                _client.cancelOrder(myBidOrders[i]);
                            }).Wait();
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Ошибка при снятии ордера!\n{e.InnerException}";
                            throw new Exception();
                        }


                        for (int j = 0; j < _myNewOrders.Count; j++)
                        {
                            if (_myNewOrders[i].Id.Equals(myBidOrders[i].Id))
                            {
                                _myNewOrders.RemoveAt(i);
                            }
                        }
                        myBidOrders.Clear();
                        continue;
                    }
                }
                catch (Exception)
                {
                    break;
                }

                for (int i = 0; i < myAskOrders.Count; i++)
                {
                    if (!(myAskOrders[i].Price <= _orderBook.Asks[0].Price + (_ident - _range - 1) * _symbol.TickSize)
                        && !(myAskOrders[i].Price >= _orderBook.Asks[0].Price + (_ident + _range + 1) * _symbol.TickSize))
                    {
                        continue;
                    }

                    Log += $"\n[{DateTime.Now.TimeOfDay}] Наш аск превысил допустимое отклонение";

                    try
                    {
                        Task.Run(() => {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Снимаем ask ордер";
                            _client.cancelOrder(myAskOrders[i]);
                        }).Wait();
                    }
                    catch (Exception e)
                    {
                        Log += $"\n[{DateTime.Now.TimeOfDay}] Ошибка при снятии ордера!\n{e.InnerException}";
                        throw new Exception();
                    }

                    for (int j = 0; j < _myNewOrders.Count; j++)
                    {
                        if (_myNewOrders[j].Id.Equals(myAskOrders[i].Id))
                        {
                            _myNewOrders.RemoveAt(j);
                        }
                    }
                    myAskOrders.Clear();
                    continue;
                }
                for (int i = 0; i < myBidOrders.Count; i++)
                {
                    if (!(myBidOrders[i].Price >= _orderBook.Bids[0].Price - (_ident - _range - 1) * _symbol.TickSize) 
                        && !(myBidOrders[i].Price <= _orderBook.Bids[0].Price - (_ident + _range + 1) * _symbol.TickSize))
                    {
                        continue;
                    }
                    
                    Log += $"\n[{DateTime.Now.TimeOfDay}] Наш бид превысил допустимое отклонение";

                    try
                    {
                        Task.Run(() => {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Снимаем bid ордер";
                            _client.cancelOrder(myBidOrders[i]);
                        }).Wait();
                    }
                    catch (Exception e)
                    {
                        Log += $"\n[{DateTime.Now.TimeOfDay}] Ошибка при снятии ордера!\n{e.InnerException}";
                        throw new Exception();
                    }
                    
                    
                    for (int j = 0; j < _myNewOrders.Count; j++)
                    {
                        if (_myNewOrders[i].Id.Equals(myBidOrders[i].Id))
                        {
                            _myNewOrders.RemoveAt(i);
                        }
                    }
                    myBidOrders.Clear();
                    continue;
                }

                if (myAskOrders.Count < 1)
                {
                    try
                    {
                        var priceValue = _orderBook.Asks[0].Price + ident * _symbol.TickSize;
                        var price = priceValue.ToString("0.00000000000");
                        price = price.Replace(',', '.');
                        var amount = _volume.ToString(CultureInfo.CurrentCulture);
                        try
                        {
                            Task.Run(() =>
                            {
                                Log += $"\n[{DateTime.Now.TimeOfDay}] открываем {OrderType.Limit} ордер, объемом {amount}, в сторону {OrderSide.Sell}, по цене {price: 0.0000000000}";
   
                                   var order = _client.openOrder(amount, OrderSide.Sell, OrderType.Limit, price);
                                _myNewOrders.Add(order);
                            }, ct).Wait(ct);
                        }
                        catch (OperationCanceledException)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            throw new Exception();
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Ошибка при открытии ордера!\n{e.InnerException}";
                            throw new Exception();
                        }
                    }
                    catch (Exception e)
                    {
                        Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                        break;
                    }
                }

                if (myBidOrders.Count < 1)
                {
                    try
                    {
                        var priceValue = _orderBook.Bids[0].Price - ident * _symbol.TickSize;
                        var price = priceValue.ToString("0.00000000000");
                        price = price.Replace(',', '.');
                        var amount = _volume.ToString(CultureInfo.CurrentCulture);
                        try
                        {
                            Task.Run(() =>
                            {
                                Log += $"\n[{DateTime.Now.TimeOfDay}] открываем {OrderType.Limit} ордер, объемом {amount}, в сторону {OrderSide.Buy}, по цене {price: 0.0000000000}";
                                var order = _client.openOrder(amount, OrderSide.Buy, OrderType.Limit, price);
                                _myNewOrders.Add(order);
                            }, ct).Wait(ct);
                        }
                        catch (OperationCanceledException)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                            throw new Exception();
                        }
                        catch (Exception e)
                        {
                            Log += $"\n[{DateTime.Now.TimeOfDay}] Ошибка при открытии ордера!\n{e.InnerException}";
                            throw new Exception();
                        }
                    }
                    catch (Exception e)
                    {
                        Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                        break;
                    }
                }
                MyOrdersCount = _openedOrders.Count;
                var vol = 0f;
                foreach (var item in _openedOrders)
                {
                    vol += item.Quantity;
                    if (item.Side.ToLower().Equals("sell")) MyAskVolume = item.Quantity;
                    if (item.Side.ToLower().Equals("buy")) MyBidVolume = item.Quantity;
                }
                MyTotalVolume = vol;
                Task.Delay(1000).Wait();
            }
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}

using MMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MMS.Api;
using System.Globalization;
using System.Diagnostics;

namespace MMS.Models
{
    public class Bot1Model : INotifyPropertyChanged
    {
        #region Notify Fields
        private string _log;
        public string Log
        {
            get { return _log; }
            private set
            {
                this.MutateVerbose(ref _log, value, RaisePropertyChanged());
                if (_log.Length > 20000)
                    this.MutateVerbose(ref _log, "", RaisePropertyChanged());
            }
        }
        public bool BotStarted {
            get { return _botStarted; }
            set
            {
                _botStarted = value;
                RaisePropertyChanged();
            }
        }
        private bool _botStarted;

        public float CurrentVolume
        {
            get { return _currentVolume; }
            set { this.MutateVerbose(ref _currentVolume, value, RaisePropertyChanged()); }
        }
        private float _currentVolume;

        public int MyMarketOrderVolume
        {
            get { return _myMarketOrderVolume; }
            set { this.MutateVerbose(ref _myMarketOrderVolume, value, RaisePropertyChanged()); }
        }
        private int _myMarketOrderVolume;

        public int OtherMarketOrderVolume
        {
            get { return _otherMarketOrderVolume; }
            set { this.MutateVerbose(ref _otherMarketOrderVolume, value, RaisePropertyChanged()); }
        }
        private int _otherMarketOrderVolume;

        public int MyTotalLimitVolume
        {
            get { return _myTotalLimitVolume; }
            private set
            {
                this.MutateVerbose(ref _myTotalLimitVolume, value, RaisePropertyChanged());
            }
        }
        private int _myTotalLimitVolume;

        public int MyLimitVolumeClosedByOtherTraders
        {
            get { return _myLimitVolumeClosedByOtherTraders; }
            private set
            {
                this.MutateVerbose(ref _myLimitVolumeClosedByOtherTraders, value, RaisePropertyChanged());
            }
        }
        private int _myLimitVolumeClosedByOtherTraders;
        #endregion

        #region Local Fields
        private IApiClient _client1;
        private IApiClient _client2;
        
        private float _targetVolume;
        private float _reservedVolume;
        private int _swapTime;
        private int _interval;

        private bool _symbolLoaded;

        private Direction _trend;

        private Symbol _symbol;
        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged;

        public Bot1Model() {

        }

        private void cancelOrders() {
            
        }

        public void stopWork() {
            //Log += $"\n[{DateTime.Now.TimeOfDay}] Отменяем ордера";
            //cancelOrders();
            Log += $"\n[{DateTime.Now.TimeOfDay}] Бот остановлен";
        }

        public void startWork(
            IApiClient client1,
            IApiClient client2,
            Direction direction,
            int targetVolume,
            int reservedVolume,
            int swapTime,
            int interval,
            CancellationToken ct)
        {
            _client1 = client1;
            _client2 = client2;

            Log = string.Empty;
            BotStarted = true;
            _trend = direction;
            _targetVolume = targetVolume;
            _reservedVolume = _targetVolume * _reservedVolume / 100f;
            _swapTime = swapTime;
            _interval = interval;

            //client1.cancelOrders();
            try
            {
                Task.Run(() => {
                    _symbolLoaded = true;
                    try
                    {
                        _symbol = _client1.getSymbol();
                    }
                    catch (Exception)
                    {
                        _symbolLoaded = false;
                        Log += $"\n[{DateTime.Now.TimeOfDay}] Не удалось загрузить информацию об инструменте.";
                        Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                        _botStarted = true;
                        BotStarted = false;
                        return;
                    }
                    if (_symbolLoaded)
                        Log += $"\n[{DateTime.Now.TimeOfDay}] информацию об инструменте полученa.";
                }, ct).Wait(ct);
            }
            catch (OperationCanceledException)
            {
                Log += $"\n[{DateTime.Now.TimeOfDay}] Внимание остановка бота.";
                return;
            }
            
            switch (_trend)
            {
                case Direction.Up:
                    try
                    {

                    }
                    catch (OperationCanceledException) { return; }
                    break;
                case Direction.Down:
                    try
                    {

                    }
                    catch (OperationCanceledException) { return; }
                    break;
                case Direction.Flat:
                    try
                    {
                        FlatMode(ct);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] Exception! => restartApp. Message: \n{e.Message}";
                        try {
                            Task.Run(() => {
                                Task.Delay(60 * 3 * 1000).Wait();
                                startWork(client1, client2, direction, targetVolume, reservedVolume, swapTime, interval, ct);
                            }, ct).Wait(ct);

                        }
                        catch (OperationCanceledException ex) 
                        {
                            Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] перезапуск отменен.";
                        }
                        return;
                    }
                    break;
            }
        }

        private void FlatMode(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested) throw new OperationCanceledException();


                MyTotalLimitVolume = 0;
                OtherMarketOrderVolume = 0;
                MyMarketOrderVolume = 0;
                MyLimitVolumeClosedByOtherTraders = 0;
                CurrentVolume = _client1.get24Volume();

                var minutesLeft = CalculateMinutesLeft();
                var localDirection = Direction.Up;
                var sideList = new List<CandleType>();
                var swapVolumeList = new List<int>();
                var targetVolume = _targetVolume - _currentVolume;
                var directionVolumes = new List<int>();
                var r = new Random();
                bool toogle = true;

                if (targetVolume <= 0)
                {
                    targetVolume = _reservedVolume;
                }
                /*
                if(targetVolume < minutesLeft * _interval * _symbol.QuantityIncrement)
                {
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] целевой обьем слишком мал. Мин значение: {_currentVolume + minutesLeft * _interval * _symbol.QuantityIncrement} токенов.";
                    return;
                }*/

                swapVolumeList = GetRandomSwapVolumes(targetVolume, minutesLeft, _swapTime, r, _symbol.QuantityIncrement);
                var deltaExtremum1 = 0f;
                var deltaExtremum = 0f;
                for (int i = 0; i < minutesLeft; i++ )
                {
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] иттератор {i}, всего {minutesLeft} иттераций.";
                    if (_currentVolume > _targetVolume && toogle)
                    {
                        Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] достигнут целевой объем, переходим на резерв.";
                        toogle = false;
                        minutesLeft = CalculateMinutesLeft();
                        targetVolume = _reservedVolume;
                        swapVolumeList = GetRandomSwapVolumes(targetVolume, minutesLeft, _swapTime, r, _symbol.QuantityIncrement);
                        i = -1;
                        continue;
                    }
                    if (i.Equals(0))
                    {
                        Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] подготавливаем систему к работе. свеча 0-я.";
                        deltaExtremum = r.Next(5, 20) / 100f;
                        deltaExtremum1 = r.Next(5, 20) / 100f;
                        localDirection = FindDirection();
                        sideList = CreateSideList(localDirection, _swapTime, r);
                        directionVolumes = GetRandomVolumes(swapVolumeList[0], _swapTime, r);
                        swapVolumeList.RemoveAt(0);
                    }
                    else if ((i % _swapTime).Equals(0))
                    {
                        Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] меняем направление, пересчитываем объемы.";
                        deltaExtremum = r.Next(5, 20) / 100f;
                        deltaExtremum1 = r.Next(5, 20) / 100f;
                        localDirection = ChangeDirection(localDirection);
                        sideList = CreateSideList(localDirection, _swapTime, r);
                        directionVolumes = GetRandomVolumes(swapVolumeList[0], _swapTime, r);
                        swapVolumeList.RemoveAt(0);
                    }

                    var oneMinutesOrdersCount = r.Next(2, 2);
                    var max = getMaximumPrice();
                    var min = getMinimumPrice();
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] начальная граница мин: {min:0.00000000}";
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] начальная граница макс: {max:0.00000000}";

                    min = (max - min) * deltaExtremum < _symbol.TickSize ? min + _symbol.TickSize : min + (max - min) * deltaExtremum;

                    max = (max - min) * deltaExtremum1 < _symbol.TickSize ? max - _symbol.TickSize : max - (max - min) * deltaExtremum1;

                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] пересчитанная граница мин: {min:0.00000000}";
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] пересчитанная граница макс: {max:0.00000000}";

                    var numbLenght = (int)Math.Abs(Math.Round(Math.Log10(_symbol.TickSize)));

                    var minuteVolumeList = GetRandomVolumesOld(directionVolumes[0], oneMinutesOrdersCount, r);
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] разбиваем объем {directionVolumes[0]:0.0} на 2 части";
                    foreach (var item in minuteVolumeList)
                    {
                        Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] {item: 0.0}";
                    }
                    directionVolumes.RemoveAt(0);
                    var side = localDirection == Direction.Up
                                    ? CandleType.Up
                                    : CandleType.Down;

                    try
                    {
                        for (int j = 0; j < oneMinutesOrdersCount; j++)
                        {
                            if (j.Equals(0))
                            {
                                Log += $"\n\n[{DateTime.UtcNow.ToShortTimeString()}] стадия открытия минутной свечи.";
                                OpenFirstOrder(minuteVolumeList[0], r, side, ct, min + (max - min) / 2, max, min);
                                minuteVolumeList.RemoveAt(0);
                            
                            }
                            else if (j == oneMinutesOrdersCount - 1)
                            {
                                Log += $"\n\n[{DateTime.UtcNow.ToShortTimeString()}] стадия формирования минутной свечи.";
                                var multiplier = Math.Pow(10,numbLenght);
                                var delta = GetRandomVolumesOld((float)((max - min) * multiplier), _swapTime + 1, r, false)[i % _swapTime];
                                OpenLastOrder(min, max, minuteVolumeList[0], i % _swapTime, delta, _swapTime, side, localDirection, r, ct);
                                sideList.RemoveAt(0);
                                minuteVolumeList.RemoveAt(0);
                                Task.Run(() => Task.Delay((60 * _interval - DateTime.UtcNow.Second) * 1000).Wait(), ct).Wait(ct);
                            }
                            else
                            {
                                Log += $"\n\n[{DateTime.UtcNow.ToShortTimeString()}] стадия закрытия минутной свечи.";
                                var volume = minuteVolumeList[0];
                                OpenMidOrder(volume, r, side, min, max, ct);
                                minuteVolumeList.RemoveAt(0);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log += "\n" + e.ToString();
                        throw new Exception();
                    }
                    CurrentVolume = _client1.get24Volume();
                    Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] Иттерация завершена. переходим к следующей.";
                }
            }
        }

        private void OpenMidOrder(int volume, Random r, CandleType candleType, float min, float max, CancellationToken ct)
        {

            Log += $"\n потоков используется:{Process.GetCurrentProcess().Threads.Count}";
            try
            {
                Log += $"\n\n[{DateTime.UtcNow.ToShortTimeString()}:{DateTime.UtcNow.Second}] мид ордер через {4} сек.";
                Task.Run(() => {
                    Task.Delay(4 * 1000).Wait();
                }, ct).Wait(ct);

                var delta = (int)((max - min) * (int)Math.Pow(10, Math.Abs(Math.Round(Math.Log10(_symbol.TickSize)))) - 100);
                var price = min + r.Next(20, 80) / 100f * delta * _symbol.TickSize;

                OpenOrder(price, volume, min + (max - min) / 2);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private void OpenLastOrder(float min, float max, int v, int i, int delta, int swapTime, CandleType candleType, Direction direction, Random r, CancellationToken ct)
        {
            try
            {
                Task.Run(() => {
                    Task.Delay(53 * 1000 - DateTime.UtcNow.Second * 1000).Wait();
                }, ct).Wait(ct);

                var price = 0f;
                if (i == swapTime - 1)
                {
                    price = candleType == CandleType.Up
                        ? max - r.Next(10, 20) * _symbol.TickSize
                        : min + r.Next(10, 20) * _symbol.TickSize;
                }
                else if (i < 2)
                {
                    price = candleType == CandleType.Up
                        ? min + (i + 1) * delta * _symbol.TickSize
                        : max - (i + 1) * delta * _symbol.TickSize;
                }
                else
                {
                    switch (direction)
                    {
                        case Direction.Up:
                                price = min + (i + 1) * delta * _symbol.TickSize;
                            break;
                        case Direction.Down:
                                price = max - (i + 1) * delta * _symbol.TickSize;
                            break;
                        case Direction.Flat:
                            break;
                    }
                }
                price = price > max ? max : price;
                price = price < min ? min : price;

                OpenOrder(price, v, min + (max - min) / 2);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private void OpenFirstOrder(int v, Random r, CandleType candleType, CancellationToken ct, float separator, float max, float min)
        {

            Log += $"\n потоков используется:{Process.GetCurrentProcess().Threads.Count}";
            try
            {
                Task.Run(() => {
                    if (DateTime.UtcNow.Second < 3) return;
                    Task.Delay(60 * 1000 - DateTime.UtcNow.Second * 1000).Wait();
                    Task.Delay(1000).Wait();
                }, ct).Wait(ct);

                var prevCandle = _client1.getCandles(2, TimeFrame.M1).CandlesCollection[0];
                var price = prevCandle.Close + r.Next(-3, 3) * _symbol.TickSize;

                price = price >= max ? max : price;
                price = price <= min ? min : price;

                OpenOrder(price, v, separator);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private void OpenOrder(float price, int v, float separator)
        {
            try
            {
                Order limit = null;
                Order market = null;

                OrderType type = OrderType.Limit;
                OrderSide side = price > separator ? OrderSide.Sell : OrderSide.Buy;

                limit = _client1.openOrder(v.ToString(), side, type, price.ToString("0.00000000"));

                Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}:{DateTime.UtcNow.Second}]Ордер: Type = {type} , Side = {side}, Val = {limit.Quantity}, Pr = {limit.Price:0.00000000}, Time {limit.CreatedAt}. Размещен";

                #region Aleksandr Liquid
                /*
                if (limit != null)
                {
                    var limit_trades = _client1.getTradesByOrder(limit.Id);
                    if (limit_trades != null && limit_trades.Trades.Count > 0)
                    {
                        Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}:{DateTime.UtcNow.Second}]Ордер: Type = {limit.Type} , Side = {limit.Side}, Val = {limit.Quantity}, Pr = {limit.Price:0.0000000000}. Закрыт кем-то";
                        _client1.cancelOrder(limit);
                        foreach (var item in limit_trades.Trades)
                        {
                            MyLimitVolumeClosedByOtherTraders += (int)item.Quantity;
                            MyTotalLimitVolume += (int)item.Quantity;
                        }
                        return;
                    }
                    MyTotalLimitVolume += (int)limit.Quantity;
                }*/
                
                #endregion
                type = OrderType.Market;
                side = price > separator ? OrderSide.Buy : OrderSide.Sell;

                market =  _client2.openOrder(v.ToString(), side, type, "");

                Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}:{DateTime.UtcNow.Second}]Ордер: Type = {type} , Side = {side}, Val = {limit.Quantity}, Pr = {limit.Price:0.00000000}, Time {limit.CreatedAt}. Размещен";
               
                #region Aleksandr Liquid
                /*if (market != null)
                {
                    var trades = _client2.getTradesByOrder(market.Id);

                    _client1.cancelOrder(limit);

                    if (trades != null && trades.Trades != null)
                    {
                        foreach (var item in trades.Trades)
                        {
                            MyMarketOrderVolume += limit.Price == item.Price
                                ? (int)item.Quantity
                                : 0;
                            OtherMarketOrderVolume += limit.Price != item.Price
                                ? (int)item.Quantity
                                : 0;
                        }
                    }
                }*/
                #endregion
            }
            catch (Exception e)
            {
                if (e.Message == "")
                {
                    Log += "\nНа ликвиде профилактика. ждем 15м";
                    Task.Run(() => Task.Delay(60 * 15 * 1000).Wait()).Wait();
                }
                else if(e.Message == "nonce")
                {
                    Log += "\nОщибка типа Nonce, открываем ордер заново.";
                    OpenOrder(price, v, separator);
                }
                else
                {
                    Log += "\n" + e.ToString();
                    throw new Exception();
                }
            }
        }
        /// <summary>
        /// разбить таргетВолюм на партКоунт разных частей.
        /// </summary>
        /// <param name="targetVolume">объем котоырй нужно рандомно разбить</param>
        /// <param name="partsCount">количество рандомных частей</param>
        /// <param name="r">Рандомайзер</param>
        /// <param name="minSize">минимальный обьем одной рандомной части</param>
        /// <returns></returns>
        private List<int> GetRandomVolumes(float targetVolume, int partsCount, Random r, bool withMin = true)
        {
            Log += $"\n Рабиваем объем {targetVolume:0.0} на {partsCount} разных частей.";
            var parts = new List<int>();
            //var surplus = 0;

            for (int i = 0; i < partsCount; i++)
            {
                float multiplier = r.Next(20, 150) / 100f;
                var volume = (int)(targetVolume / partsCount * multiplier);
                volume -= volume % 5;
                volume = volume < (int)_symbol.QuantityIncrement ? (int)_symbol.QuantityIncrement : volume;
                Log += $"\n{volume}";
                parts.Add(volume);
                //surplus += volume;
            }
            /*
            surplus = ((int)targetVolume - surplus) / partsCount;

            for (int i = 0; i < partsCount; i++)
            {
                parts[i] += surplus;
                if (withMin)
                {
                    parts[i] -= parts[i] % 5;
                    if (parts[i] < _symbol.QuantityIncrement)
                    {
                        parts[i] = (int)_symbol.QuantityIncrement;
                    }
                }
            }*/
            return parts;
        }

        private List<int> GetRandomVolumesOld(float targetVolume, int partsCount, Random r, bool withMin = true)
        {
            var parts = new List<int>();
            var surplus = 0;

            for (int i = 0; i < partsCount; i++)
            {
                float multiplier = r.Next(20, 80) / 100f;
                var volume = (int)(targetVolume / partsCount * multiplier);
                parts.Add(volume);
                surplus += volume;
            }
            
            surplus = ((int)targetVolume - surplus) / partsCount;

            for (int i = 0; i < partsCount; i++)
            {
                parts[i] += surplus;
                if (withMin)
                {
                    if (parts[i] < _symbol.QuantityIncrement)
                    {
                        parts[i] = (int)_symbol.QuantityIncrement;
                    }
                }
            }
            return parts;
        }

        private List<int> GetRandomSwapVolumes(float targetVolume, int minutesLeft, int swapTime, Random r, float quoteIncrement)
        {
            var fullPartsCount = minutesLeft / swapTime;
            if (minutesLeft / (float)swapTime > 0) fullPartsCount++;
            return GetRandomVolumes(targetVolume, fullPartsCount, r);
        }

        private float getMinimumPrice()
        {
            var result = _client1.getOrderBook().Bids[0].Price;
            return result;
        }

        private float getMaximumPrice()
        {
            var result = _client1.getOrderBook().Asks[0].Price;
            return result;
        }

        private Direction ChangeDirection(Direction localDirection)
        {
            var result = localDirection.Equals(Direction.Up)
                ? Direction.Down
                : Direction.Up;

            Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] меняем направление с {localDirection.ToString()} на {result.ToString()}";
            return result;
        }

        private List<CandleType> CreateSideList(Direction localDirection, int range, Random r)
        {
            var strong = r.Next(40, 80) / 100f;
            var result = new List<CandleType>();
            var mainPart = Math.Ceiling(range * strong);
            var additionalPart = range - mainPart;
            for (int i = 0; i < range; )
            {
                if (i < 2)
                {
                    result.Add(
                        localDirection.Equals(Direction.Up)
                        ? CandleType.Up
                        : CandleType.Down
                        );
                    mainPart--;
                    i++;
                    continue;
                }
                if (r.Next(0, 2).Equals(0))
                {
                    if (mainPart.Equals(0)) continue;

                    result.Add(
                        localDirection.Equals(Direction.Up)
                        ? CandleType.Up
                        : CandleType.Down
                        );
                    mainPart--;
                    i++;
                }
                else
                {
                    if (additionalPart.Equals(0)) continue;

                    result.Add(
                        localDirection.Equals(Direction.Up)
                        ? CandleType.Down
                        : CandleType.Up
                        );
                    additionalPart--;
                    i++;
                }
            }
            return result;
        }

        private Direction FindDirection()
        {
            var orderBook = _client1.getOrderBook();
            if (orderBook.Asks.Count == 0) throw new Exception("в ордербуке нету асков");
            if (orderBook.Bids.Count == 0) throw new Exception("в ордербуке нету бидов");
            var askElem = orderBook.Asks[0] ?? throw new ArgumentNullException(nameof(orderBook.Asks));
            var bidElem = orderBook.Bids[0] ?? throw new ArgumentNullException(nameof(orderBook.Bids));

            var max = askElem.Price;
            var min = bidElem.Price;

            var close = _client1.getCandles(timeFrame:TimeFrame.M1).CandlesCollection[0].Close;

            var result = max - close < close - min
                ? Direction.Down
                : Direction.Up;

            Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] движемся в {result.ToString()}";

            return result;
        }

        private int CalculateMinutesLeft()
        {
            var now = DateTime.UtcNow;
            var nextDay = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
            var result = (int)(nextDay - now).TotalMinutes / 2;
            Log += $"\n[{DateTime.UtcNow.ToShortTimeString()}] дневная свеча закроется через {result} минут(у)";
            return result;
        }
        
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
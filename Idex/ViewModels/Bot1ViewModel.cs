using DevExpress.Mvvm;
using MMS.Api;
using MMS.Domain;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MMS.ViewModels
{
    public class Bot1ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AccountItem> AccountCollection => HomeViewModel.AccountItems;

        public AccountItem Account1
        {
            get { return _account1; }
            set
            {
                this.MutateVerbose(ref _account1, value, RaisePropertyChanged());
            }
        }
        private AccountItem _account1;
        public AccountItem Account2
        {
            get { return _account2; }
            set
            {
                this.MutateVerbose(ref _account2, value, RaisePropertyChanged());
            }
        }
        private AccountItem _account2;

        private Direction _direction;
        public Direction direction {
            get { return _direction; }
            set
            {
                this.MutateVerbose(ref _direction, value, RaisePropertyChanged());
            }
        }
        public Visibility WaitNextDay
        {
            get { return _waitNextDay; }
            set { this.MutateVerbose(ref _waitNextDay, value, RaisePropertyChanged()); }
        }
        private Visibility _waitNextDay;

        private int _hoursUntilNextDay;
        public int HoursUntilNextDay
        {
            get { return _hoursUntilNextDay; }
            set { this.MutateVerbose(ref _hoursUntilNextDay, value, RaisePropertyChanged()); }
        }
        private int _minutesUntilNextHour;
        public int MinutesUntilNextHour
        {
            get { return _minutesUntilNextHour; }
            set { this.MutateVerbose(ref _minutesUntilNextHour, value, RaisePropertyChanged()); }
        }
        private int _targetVolume;
        public int TargetVolume {
            get { return _targetVolume; }
            set
            {
                this.MutateVerbose(ref _targetVolume, value, RaisePropertyChanged());
            }
        }
        private int _directionRatio;
        public int directionRatio
        {
            get { return _directionRatio; }
            set
            {
                this.MutateVerbose(ref _directionRatio, value, RaisePropertyChanged());
            }
        }

        private int _reservedVolume;
        public int ReservedVolume
        {
            get { return _reservedVolume; }
            set
            {
                this.MutateVerbose(ref _reservedVolume, value, RaisePropertyChanged());
            }
        }
        private int _swapTime;
        public int SwapTime
        {
            get { return _swapTime; }
            set { this.MutateVerbose(ref _swapTime, value, RaisePropertyChanged()); }
        }

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set { this.MutateVerbose(ref _interval, value, RaisePropertyChanged()); }
        }

        private string _log;
        public string Log {
            get { return _log; }
            set {
                this.MutateVerbose(ref _log, value, RaisePropertyChanged());
            }
        }

        private float _missingPurchases;
        public float MissingPurchases
        {
            get { return _missingPurchases; }
            private set
            {
                this.MutateVerbose(ref _missingPurchases, value, RaisePropertyChanged());
            }
        }
        private float _missingSales;
        public float MissingSales
        {
            get { return _missingSales; }
            private set
            {
                this.MutateVerbose(ref _missingSales, value, RaisePropertyChanged());
            }
        }
        private float _myTotalLimitVolume;
        public float MyTotalLimitVolume
        {
            get { return _myTotalLimitVolume; }
            private set
            {
                this.MutateVerbose(ref _myTotalLimitVolume, value, RaisePropertyChanged());
            }
        }
        private float _totalVolume;
        public float TotalVolume
        {
            get { return _totalVolume; }
            private set
            {
                this.MutateVerbose(ref _totalVolume, value, RaisePropertyChanged());
            }
        }
        private float _myLimitVolumeClosedByOtherTraders;
        public float MyLimitVolumeClosedByOtherTraders
        {
            get { return _myLimitVolumeClosedByOtherTraders; }
            private set
            {
                this.MutateVerbose(ref _myLimitVolumeClosedByOtherTraders, value, RaisePropertyChanged());
            }
        }
        
        public float CurrentVolume
        {
            get { return _currentVolume; }
            set { this.MutateVerbose(ref _currentVolume, value, RaisePropertyChanged()); }
        }
        private float _currentVolume;
        public int MyMarketOrderVolume
        {
            get { return _myLimitOrderVolume; }
            set { this.MutateVerbose(ref _myLimitOrderVolume, value, RaisePropertyChanged()); }
        }
        private int _myLimitOrderVolume;
        public int OtherMarketOrderVolume
        {
            get { return _otherLimitOrderVolume; }
            set { this.MutateVerbose(ref _otherLimitOrderVolume, value, RaisePropertyChanged()); }
        }
        private int _otherLimitOrderVolume;
        private bool _unLockInput;
        public bool UnLockInput
        {
            get { return _unLockInput; }
            set
            {
                this.MutateVerbose(ref _unLockInput, value, RaisePropertyChanged());
            }
        }
        private bool _botStarted;
        public bool BotStarted
        {
            get { return _botStarted; }
            set
            {
                if (value == UnLockInput)
                {
                    UnLockInput = !value;
                }
                this.MutateVerbose(ref _botStarted, value, RaisePropertyChanged());
            }
        }

        public bool hideSlider => direction != Direction.Flat && UnLockInput;

        public IList<int> stopValueList { get; }
        public IList<Enum> directionList { get; }
        public IList<int> priceShiftValuesList { get; }
        

        readonly Bot1Model _model = new Bot1Model();
        private CancellationTokenSource _ts;
        private CancellationToken _ct;
        private Task _startTask;
        private Task _stopTask;
        private Action _startAction;
        private Action _stopAction;

        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (_startTask == null || _startTask.IsCompleted)
                    {
                        Log += $"\nпоток - {Thread.CurrentThread.ManagedThreadId}, предыдущая главная задача была завершена либо ее не существует. Создаем новую задачу.";

                        _ts = new CancellationTokenSource();
                        _ct = _ts.Token;
                        _startTask = new Task(_startAction, _ct);
                    }
                    _startTask.Start();
                }, () => (_stopTask == null || _stopTask.IsCompleted) && _startTask?.Status != TaskStatus.Running);
            }
        }
        public ICommand StopCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (_startTask == null || _startTask.Status != TaskStatus.Running)
                    {
                        Log += $"\nпоток - {Thread.CurrentThread.ManagedThreadId}, задача, которую мы хотим отменить либо не запущена, либо ее не существует";
                        Log += $"\nпоток - {Thread.CurrentThread.ManagedThreadId}, сущаствует ли? {_startTask != null}";
                        Log += $"\nпоток - {Thread.CurrentThread.ManagedThreadId}, запущена ли? {_startTask.Status == TaskStatus.Running}";
                        return;
                    }
                    if (_stopTask == null || _stopTask.IsCompleted)
                    {
                        Log += $"\nпоток - {Thread.CurrentThread.ManagedThreadId}, предыдущая задача остановки была завершена либо ее не существует. Создаем новую задачу.";
                        _stopTask = new Task(_stopAction);
                    }
                    if (_stopTask.Status != TaskStatus.Running)
                    {
                        _stopTask.Start();
                    }
                    else
                    {
                        Log += $"\nпоток - {Thread.CurrentThread.ManagedThreadId}, не могу отменить задачу. операция отмены уже запущена";
                    }
                }, () => _startTask?.Status == TaskStatus.Running);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public Bot1ViewModel(PairName pairName)
        {
            WaitNextDay = Visibility.Hidden;
            MyTotalLimitVolume = 0;
            MissingPurchases = 0;
            MissingSales = 0;
            UnLockInput = true;
            directionList = new List<Enum>() {
                Direction.Up,
                Direction.Down,
                Direction.Flat
            };
            stopValueList = new List<int>(Enumerable.Range(1, 10));
            priceShiftValuesList = new List<int>(Enumerable.Range(1, 10));
            Interval = 1;

            _direction = Direction.Flat;
            TargetVolume = 1000000;
            ReservedVolume = 10;
            SwapTime = 60;
            Interval = 1;

            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "Log") Log = (s as Bot1Model).Log; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "BotStarted") BotStarted = (s as Bot1Model).BotStarted; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "CurrentVolume") CurrentVolume = (s as Bot1Model).CurrentVolume; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "MyMarketOrderVolume") MyMarketOrderVolume = (s as Bot1Model).MyMarketOrderVolume; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "MyLimitVolumeClosedByOtherTraders") MyLimitVolumeClosedByOtherTraders = (s as Bot1Model).MyLimitVolumeClosedByOtherTraders; };
            _model.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "MyTotalLimitVolume") MyTotalLimitVolume = (s as Bot1Model).MyTotalLimitVolume;
                TotalVolume = MyTotalLimitVolume + OtherMarketOrderVolume;
            };
            _model.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "OtherMarketOrderVolume") OtherMarketOrderVolume = (s as Bot1Model).OtherMarketOrderVolume;
                TotalVolume = MyTotalLimitVolume + OtherMarketOrderVolume;
            };

            _startAction = new Action(() => {
                BotStarted = true;
                if (Account1.SelectedPlatform != Account2.SelectedPlatform)
                {
                    Log += "\nОшибка. торговая площадка на первом аккаунте отличается от торговой площадки на втором аккаунте.";
                    return;
                }
                switch (Account1.SelectedPlatform)
                {
                    case Platform.HitBTC:
                        _model.StartWork(
                            new HitBtcClient(Account1.ApiKey, Account1.ApiSecret, pairName),
                            new HitBtcClient(Account2.ApiKey, Account2.ApiSecret, pairName),
                            _direction,
                            _targetVolume,
                            _swapTime,
                            Interval,
                            _ct);
                        break;
                    case Platform.Bistox:
                        _model.StartWork(
                            new BistoxClient(Account1.ApiKey, Account1.ApiSecret, pairName),
                            new BistoxClient(Account2.ApiKey, Account2.ApiSecret, pairName),
                            _direction,
                            _targetVolume,
                            _swapTime,
                            Interval,
                            _ct);
                        break;
                    default:
                        _model.StartWork(
                            new LiquidClient(Account1.ApiKey, Account1.ApiSecret, pairName),
                            new LiquidClient(Account2.ApiKey, Account2.ApiSecret, pairName),
                            _direction,
                            _targetVolume,
                            _swapTime,
                            Interval,
                            _ct);
                        break;
                }
            });
            _stopAction = new Action(() => {
                _ts.Cancel();
                Task.WaitAny(_startTask);
                Task.Run(() => {
                    _model.StopWork();
                }).Wait();
                BotStarted = false;
                RaisePropertyChanged();
                return;
            });
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}

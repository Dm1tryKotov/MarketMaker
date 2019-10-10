using DevExpress.Mvvm;
using MMS.Api;
using MMS.Domain;
using MMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MMS.ViewModels
{
    public class Bot2ViewModel : INotifyPropertyChanged
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
        private Direction _direction;
        public Direction Direction
        {
            get { return _direction; }
            set
            {
                this.MutateVerbose(ref _direction, value, RaisePropertyChanged());
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



        private int _volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                this.MutateVerbose(ref _volume, value, RaisePropertyChanged());
            }
        }
        private int _depth;
        public int Depth
        {
            get { return _depth; }
            set
            {
                this.MutateVerbose(ref _depth, value, RaisePropertyChanged());
            }
        }
        private int _ident;
        public int Ident
        {
            get { return _ident; }
            set
            {
                this.MutateVerbose(ref _ident, value, RaisePropertyChanged());
            }
        }
        private int _range;
        public int Range
        {
            get { return _range; }
            set
            {
                this.MutateVerbose(ref _range, value, RaisePropertyChanged());
            }
        }
        private int _orderSpace;
        public int OrderSpace
        {
            get { return _orderSpace; }
            set
            {
                this.MutateVerbose(ref _orderSpace, value, RaisePropertyChanged());
            }
        }
        private int _directionStrong;
        public int DirectionStrong
        {
            get { return _directionStrong; }
            set
            {
                this.MutateVerbose(ref _directionStrong, value, RaisePropertyChanged());
            }
        }

        private string _log;
        public string Log
        {
            get { return _log; }
            set
            {
                this.MutateVerbose(ref _log, value, RaisePropertyChanged());
            }
        }

        private bool _unLockInput;
        public bool UnLockInput
        {
            get { return _unLockInput; }
            set
            {
                this.MutateVerbose(ref _unLockInput, value, RaisePropertyChanged());
            }
        }

        public bool HideSlider => Direction != Direction.Flat && UnLockInput;

        public IList<Direction> DirectionList { get; }

        readonly Bot2Model _model = new Bot2Model();
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

        public Bot2ViewModel(PairName pairName)
        {
            DirectionStrong = 51;
            UnLockInput = true;
            var directionArray = Enum.GetValues(typeof(Direction));
            DirectionList = new List<Direction>();
            foreach (Direction item in directionArray)
            {
                DirectionList.Add(item);
            }

            _startAction = new Action(() => {
                switch (Account1.SelectedPlatform)
                {
                    case Platform.HitBTC:
                        _model.startWork(
                            new HitBtcClient(Account1.ApiKey, Account1.ApiSecret, pairName),
                            Direction, Ident, Depth, Volume,
                            OrderSpace, DirectionStrong, Range, _ct);
                        break;
                }

            });
            _stopAction = new Action(() => {
                _ts.Cancel();
                Task.WaitAny(_startTask);
                Task.Run(() => {
                    _model.stopWork();
                }).Wait();
                RaisePropertyChanged();
                return;
            });
            
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("Log")) Log = (s as Bot2Model).Log; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("MyAskVolume")) MyAskVolume = (s as Bot2Model).MyAskVolume; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("MyBidVolume")) MyBidVolume = (s as Bot2Model).MyBidVolume; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("MyOrdersCount")) MyOrdersCount = (s as Bot2Model).MyOrdersCount; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("MyTotalVolume")) MyTotalVolume = (s as Bot2Model).MyTotalVolume; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("MyBidsCount")) MyBidsCount = (s as Bot2Model).MyBidsCount; };
            _model.PropertyChanged += (s, e) => { if (e.PropertyName.Equals("MyAsksCount")) MyAsksCount = (s as Bot2Model).MyAsksCount; };
        }
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}

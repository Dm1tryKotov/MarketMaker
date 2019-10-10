using MaterialDesignThemes.Wpf;
using MMS.Views;
using MMS.ViewModels;
using System;
using MMS.Models;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MMS.ViewModels
{
    public class Bot1ContentViewModel
    {
        public ObservableCollection<MenuItem> MenuItems => _menuItem;
        private ObservableCollection<MenuItem> _menuItem;

        public Bot1ContentViewModel()
        {
            _menuItem = new ObservableCollection<MenuItem>()
            {
                new MenuItem("ETH/XRP", new Bot1 { DataContext = new Bot1ViewModel(PairName.ETHXRP) }),
                new MenuItem("ETH/XLM", new Bot1 { DataContext = new Bot1ViewModel(PairName.ETHXLM) }),
                new MenuItem("ETH/BCH", new Bot1 { DataContext = new Bot1ViewModel(PairName.ETHBCH) }),
                new MenuItem("ETH/LTC", new Bot1 { DataContext = new Bot1ViewModel(PairName.ETHLTC) }),

                new MenuItem("BTC/BCH", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCBCH) }),
                new MenuItem("BTC/ETH", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCETH) }),
                new MenuItem("BTC/LTC", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCLTC) }),
                new MenuItem("BTC/XEM", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCXEM) }),
                new MenuItem("BTC/XLM", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCXLM) }),
                new MenuItem("BTC/XRP", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCXRP) }),
            };
        }

        public ICommand AddNewTab {
            get {
                return new DelegateCommand(()=> {
                    _menuItem.Add(new MenuItem("ETH/XLM", new Bot1 { DataContext = new Bot1ViewModel(PairName.ETHXLM) }));
                }, ()=> MenuItems.Count != 2);
            }
        }
    }
}

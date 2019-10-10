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
                new MenuItem("ADH/BTC", new Bot1 { DataContext = new Bot1ViewModel(PairName.BTCADH) }),
                new MenuItem("ADH/ETH", new Bot1 { DataContext = new Bot1ViewModel(PairName.ETHADH) })
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

using DevExpress.Mvvm;
using MMS.Models;
using MMS.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MMS.ViewModels
{
    public class Bot2ContentViewModel
    {
        public ObservableCollection<MenuItem> MenuItems => _menuItem;
        private ObservableCollection<MenuItem> _menuItem;

        public Bot2ContentViewModel()
        {
            _menuItem = new ObservableCollection<MenuItem>()
            {
                new MenuItem("ADH/ETH", new Bot2 { DataContext = new Bot2ViewModel(PairName.ETHADH) })
            };
        }

        public ICommand AddNewTab
        {
            get
            {
                return new DelegateCommand(() => {
                    _menuItem.Add(new MenuItem("ADH/BTC", new Bot2 { DataContext = new Bot2ViewModel(PairName.BTCADH) }));
                }, () => MenuItems.Count != 2);
            }
        }
    }
}

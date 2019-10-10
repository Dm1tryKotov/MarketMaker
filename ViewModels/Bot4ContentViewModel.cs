using DevExpress.Mvvm;
using MMS.Models;
using MMS.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace MMS.ViewModels
{
    public class Bot4ContentViewModel
    {
        public ObservableCollection<MenuItem> MenuItems => _menuItem;
        private ObservableCollection<MenuItem> _menuItem;

        public Bot4ContentViewModel()
        {
            _menuItem = new ObservableCollection<MenuItem>()
            {
                new MenuItem("ADH/ETH", new Bot4 { DataContext = new Bot4ViewModel(PairName.ETHADH) })
            };
        }

        public ICommand AddNewTab
        {
            get
            {
                return new DelegateCommand(() => {
                    _menuItem.Add(new MenuItem("ADH/BTC", new Bot4 { DataContext = new Bot4ViewModel(PairName.BTCADH) }));
                }, () => MenuItems.Count != 2);
            }
        }
    }
}

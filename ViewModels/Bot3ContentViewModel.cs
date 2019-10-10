using DevExpress.Mvvm;
using MMS.Models;
using MMS.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace MMS.ViewModels
{
    public class Bot3ContentViewModel
    {
        public ObservableCollection<MenuItem> MenuItems => _menuItem;
        private ObservableCollection<MenuItem> _menuItem;

        public Bot3ContentViewModel()
        {
            _menuItem = new ObservableCollection<MenuItem>()
            {
                new MenuItem("ADH/ETH", new Bot3 { DataContext = new Bot3ViewModel(PairName.ETHADH) })
            };
        }

        public ICommand AddNewTab
        {
            get
            {
                return new DelegateCommand(() => {
                    _menuItem.Add(new MenuItem("ADH/BTC", new Bot3 { DataContext = new Bot3ViewModel(PairName.BTCADH) }));
                }, () => MenuItems.Count != 2);
            }
        }
    }
}

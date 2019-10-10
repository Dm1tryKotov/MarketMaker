using DevExpress.Mvvm;
using MMS.Models;
using MMS.Views;
using MaterialDesignThemes.Wpf;
using System;

namespace MMS.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public MainViewModel(ISnackbarMessageQueue snackbarMessageQueue) {
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            menuItems = new[]
            {
                new MenuItem("На главную", new Home(){ DataContext = new HomeViewModel() }),
                new MenuItem("Бот 1", new Bot1TabControl { DataContext = new Bot1ContentViewModel() }),
                new MenuItem("Бот 2", new Bot2TabControl { DataContext = new Bot2ContentViewModel() }),
                new MenuItem("Бот 3", new Bot3TabControl { DataContext = new Bot3ContentViewModel() }),
                new MenuItem("Бот 4", new Bot4TabControl { DataContext = new Bot4ContentViewModel() }),
            };
        }
        public MenuItem[] menuItems { get; }
    }
}

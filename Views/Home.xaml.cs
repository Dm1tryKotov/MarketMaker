using MaterialDesignThemes.Wpf;
using MMS.Models;
using MMS.ViewModels;
using System.Windows.Controls;

namespace MMS.Views
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //you can cancel the dialog close:
            //eventArgs.Cancel();

            if (!Equals(eventArgs.Parameter, true)) return;

            Platform p = (Platform)Platform_combo_box.SelectedItem;

            (DataContext as HomeViewModel).AddNewAccount(ApiKey_tb.Text, ApiSecret_tb.Text, p);
        }
    }
   
}

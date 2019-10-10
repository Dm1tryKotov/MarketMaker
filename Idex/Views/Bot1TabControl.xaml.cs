using MMS.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MMS.Views
{
    /// <summary>
    /// Interaction logic for Bot1TabControl.xaml
    /// </summary>
    public partial class Bot1TabControl : UserControl
    {
        public Bot1TabControl()
        {
            InitializeComponent();

            DataContext = new Bot1ContentViewModel();
        }
    }
}

using NPhoenixSPA.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NPhoenixSPA.Pages
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Page
    {
        public Account(AccountViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}

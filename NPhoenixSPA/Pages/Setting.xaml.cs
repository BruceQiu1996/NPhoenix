using NPhoenixSPA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NPhoenixSPA.Pages
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Page
    {
        public Setting(SettingViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Down)
                e.Handled = true;
        }
    }
}

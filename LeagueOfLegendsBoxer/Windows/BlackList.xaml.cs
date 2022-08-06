using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for BlackList.xaml
    /// </summary>
    public partial class BlackList : Window
    {
        public BlackList(BlackListViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }
    }
}

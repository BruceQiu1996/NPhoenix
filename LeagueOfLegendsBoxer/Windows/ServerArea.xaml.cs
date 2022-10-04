using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for ServerArea.xaml
    /// </summary>
    public partial class ServerArea : Window
    {
        public ServerArea(ServerAreaViewModel serverAreaViewModel)
        {
            InitializeComponent();
            DataContext = serverAreaViewModel;
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

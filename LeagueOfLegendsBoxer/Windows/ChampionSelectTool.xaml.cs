using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for ChampionSelectTool.xaml
    /// </summary>
    public partial class ChampionSelectTool : Window
    {
        public ChampionSelectTool(ChampionSelectToolViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}

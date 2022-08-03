using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for Team1V2Window.xaml
    /// </summary>
    public partial class Team1V2Window : Window
    {
        public Team1V2Window(Team1V2WindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

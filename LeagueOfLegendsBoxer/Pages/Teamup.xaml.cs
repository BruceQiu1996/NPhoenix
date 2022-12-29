using LeagueOfLegendsBoxer.ViewModels.Pages;
using System.Windows.Controls;

namespace LeagueOfLegendsBoxer.Pages
{
    /// <summary>
    /// Interaction logic for Teamup.xaml
    /// </summary>
    public partial class Teamup : Page
    {
        public Teamup(TeamupViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

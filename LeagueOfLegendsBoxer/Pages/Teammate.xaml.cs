using LeagueOfLegendsBoxer.ViewModels.Pages;
using System.Windows.Controls;

namespace LeagueOfLegendsBoxer.Pages
{
    /// <summary>
    /// Interaction logic for Teammate.xaml
    /// </summary>
    public partial class Teammate : Page
    {
        public Teammate(TeammateViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}

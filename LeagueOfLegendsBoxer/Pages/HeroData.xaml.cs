using LeagueOfLegendsBoxer.ViewModels.Pages;
using System.Windows.Controls;

namespace LeagueOfLegendsBoxer.Pages
{
    /// <summary>
    /// Interaction logic for HeroData.xaml
    /// </summary>
    public partial class HeroData : Page
    {
        public HeroData(HeroDataViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

using LeagueOfLegendsBoxer.ViewModels.Pages;
using System.Windows.Controls;

namespace LeagueOfLegendsBoxer.Pages
{
    /// <summary>
    /// Interaction logic for Rank.xaml
    /// </summary>
    public partial class RecordRank : Page
    {
        public RecordRank(RecordRankViewModel recordRankViewModel)
        {
            InitializeComponent();
            DataContext = recordRankViewModel;
        }
    }
}
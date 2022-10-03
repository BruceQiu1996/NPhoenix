using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for Rank.xaml
    /// </summary>
    public partial class RecordRank : Window
    {
        public RecordRank(RecordRankViewModel recordRankViewModel)
        {
            InitializeComponent();
            DataContext = recordRankViewModel;
        }

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hide();
        }
    }
}

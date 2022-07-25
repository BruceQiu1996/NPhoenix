using System.Windows;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for SummonerAnalyse.xaml
    /// </summary>
    public partial class SummonerAnalyse : Window
    {
        public SummonerAnalyse()
        {
            InitializeComponent();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
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

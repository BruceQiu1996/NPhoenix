using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LeagueOfLegendsBoxer.UserControls
{
    /// <summary>
    /// Interaction logic for SVP.xaml
    /// </summary>
    public partial class SVP : UserControl
    {
        public static DependencyProperty MyFontSizeProperty = DependencyProperty.Register("MyFontSize", typeof(double), typeof(MVP));
        public double MyFontSize
        {
            get { return (double)GetValue(MyFontSizeProperty); }
            set { SetValue(MyFontSizeProperty, value); }
        }

        public SVP()
        {
            InitializeComponent();
            txt.SetBinding(FontSizeProperty, new Binding("MyFontSize") { Source = this });
        }
    }
}

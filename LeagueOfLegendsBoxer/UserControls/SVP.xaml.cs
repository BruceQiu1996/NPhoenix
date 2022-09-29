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
        public static DependencyProperty MyFontSize1Property = DependencyProperty.Register("MyFontSize1", typeof(double), typeof(MVP));
        public double MyFontSize1
        {
            get { return (double)GetValue(MyFontSize1Property); }
            set { SetValue(MyFontSize1Property, value); }
        }

        public SVP()
        {
            InitializeComponent();
            txt.SetBinding(FontSizeProperty, new Binding("MyFontSize1") { Source = this });
        }
    }
}

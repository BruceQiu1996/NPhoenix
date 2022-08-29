using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeagueOfLegendsBoxer.Converts
{
    public class BoolColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool temp = (bool)value;
            if (temp)
                return System.Windows.Application.Current.FindResource("PrimaryBrush");
            else
            {
                if (App.CURRENT_THEME == App.Theme.Light)
                    return new SolidColorBrush(Color.FromRgb(0, 0, 0));
                else
                    return new SolidColorBrush(Color.FromRgb(240, 230, 210));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolColorBrushReConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool temp = (bool)value;
            if (!temp)
                return System.Windows.Application.Current.FindResource("PrimaryBrush");
            else
            {
                if (App.CURRENT_THEME == App.Theme.Light)
                    return new SolidColorBrush(Color.FromRgb(0, 0, 0));
                else
                    return new SolidColorBrush(Color.FromRgb(240, 230, 210));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

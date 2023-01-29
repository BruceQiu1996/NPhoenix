using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeagueOfLegendsBoxer.Converts
{
    public class StringValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? val = double.TryParse(value?.ToString(), out var temp) ? temp : null;
            if (val == null)
                return null;

            if (val > 0)
                return new SolidColorBrush(Color.FromRgb(0,139,69));

            else if (val < 0)
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));

            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

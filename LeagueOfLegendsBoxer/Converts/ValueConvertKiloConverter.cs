using System;
using System.Globalization;
using System.Windows.Data;

namespace LeagueOfLegendsBoxer.Converts
{
    public class ValueConvertKiloConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (int)value;
            return (data * 1.0 / 1000).ToString("0.0") + "k";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

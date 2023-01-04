using System;
using System.Globalization;
using System.Windows.Data;

namespace LeagueOfLegendsBoxer.Converts
{
    public class ValueHalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;
            return temp / 1.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

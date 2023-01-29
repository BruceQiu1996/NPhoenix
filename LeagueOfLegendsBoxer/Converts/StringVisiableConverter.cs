using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LeagueOfLegendsBoxer.Converts
{
    public class StringVisiableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(string.IsNullOrEmpty(value?.ToString()))
                return Visibility.Collapsed;

            double? val = double.TryParse(value?.ToString(), out var temp) ? temp : null;
            if (val == null)
                return Visibility.Collapsed;

            if(val==0)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

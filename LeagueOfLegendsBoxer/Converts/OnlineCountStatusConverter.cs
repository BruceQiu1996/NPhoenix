using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeagueOfLegendsBoxer.Converts
{
    public class OnlineCountStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = (int)value;
            if (count > 600)
            {
                return "爆满";
            }
            else if (count > 300)
            {
                return "拥挤";
            }
            else
            {
                return "空闲";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OnlineCountColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = (int)value;
            if (count > 600)
            {
                return new SolidColorBrush(Color.FromRgb(178,34,34));
            }
            else if (count > 300)
            {
                return new SolidColorBrush(Color.FromRgb(255,140,0));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(34,139,34));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

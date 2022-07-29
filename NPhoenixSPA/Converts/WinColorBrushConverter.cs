using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NPhoenixSPA.Converts
{
    public class WinColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool temp = (bool)value;
            if (temp)
                return new SolidColorBrush(Color.FromRgb(0, 139, 69));
            else
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace NPhoenixSPA.Converts
{
    public class WinTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool temp = (bool)value;
            if (temp)
                return "胜利";
            else
                return "失败";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

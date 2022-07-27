using System;
using System.Globalization;
using System.Windows.Data;

namespace LeagueOfLegendsBoxer.Converts
{
    public class ConnectedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool temp = (bool)value;
            if (temp)
                return "已连接";
            else
                return "未连接,请启动游戏";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using LeagueOfLegendsBoxer.Resources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LeagueOfLegendsBoxer.Converts
{
    public class HorseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return new SolidColorBrush(Color.FromRgb(105, 105, 105));

            var model = App.ServiceProvider.GetRequiredService<IniSettingsModel>();
            var horse = value.ToString();
            if (horse == "未知的马")
            {
                return new SolidColorBrush(Color.FromRgb(105, 105, 105));
            }
            else if (horse == model.Above120ScoreTxt)
            {
                return new SolidColorBrush(Color.FromRgb(255, 193, 37));
            }
            else if (horse == model.Above110ScoreTxt)
            {
                return new SolidColorBrush(Color.FromRgb(205, 85, 85));
            }
            else if (horse == model.Above100ScoreTxt)
            {
                return new SolidColorBrush(Color.FromRgb(0, 205, 205));
            }
            else if (horse == model.Below100ScoreTxt)
            {
                return new SolidColorBrush(Color.FromRgb(46, 139, 87));
            }
            else 
            {
                return new SolidColorBrush(Color.FromRgb(105, 105, 105));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

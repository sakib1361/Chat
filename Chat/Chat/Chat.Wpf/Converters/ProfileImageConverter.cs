using ChatEngine.Services;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Chat.Wpf.Converters
{
    public class ProfileImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string username)
            {
                return AppService.GenerateIcon(username);
            }
            else
            {
                return AppService.GenerateIcon("Default");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

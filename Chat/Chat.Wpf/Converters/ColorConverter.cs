using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Chat.Wpf.Converters
{
    public class BoolColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool data && data)
                return Brushes.LightGreen;
            else return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

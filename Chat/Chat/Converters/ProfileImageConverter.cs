using Chat.Helpers;
using ChatClient.Services;
using Jdenticon;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Chat.Converters
{
    internal class ProfileImageConverter : IValueConverter
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

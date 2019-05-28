using Chat.Helpers;
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
                return GenerateIcon(username);
            }
            else
            {
                return GenerateIcon("Default");
            }
        }

        private string GenerateIcon(string username)
        {
            var tmpdir = System.IO.Path.GetTempPath();
            var appFolder = Path.Combine(tmpdir, AppConstants.AppName);
            Directory.CreateDirectory(appFolder);

            var imgFile = Path.Combine(appFolder, username + ".png");
            if (File.Exists(imgFile) == false)
            {
                Identicon.FromValue(username, 160).SaveAsPng(imgFile);
            }
            return imgFile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

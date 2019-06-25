using ChatCore.Engine;
using ChatClient.Helpers;
using ChatClient.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Jdenticon;
using System.IO;

namespace ChatClient.Services
{
    public static class AppService
    {
        public static string CurrentUser;

        public static void Register()
        {
            SimpleIoc.Default.Register<ClientHandler>();
            SimpleIoc.Default.Register<ChatService>();

            SimpleIoc.Default.Register<HomePageModel>();
            SimpleIoc.Default.Register<ChatPageModel>();
            SimpleIoc.Default.Register<LoginPageModel>();
            SimpleIoc.Default.Register<RegisterPageModel>();
            SimpleIoc.Default.Register<ServerPageModel>();
        }

        public static string GenerateIcon(string username)
        {
            var tmpdir = Path.GetTempPath();
            var appFolder = Path.Combine(tmpdir, AppConstants.AppName);
            Directory.CreateDirectory(appFolder);

            var imgFile = Path.Combine(appFolder, username + ".png");
            if (File.Exists(imgFile) == false)
            {
                Identicon.FromValue(username, 160).SaveAsPng(imgFile);
            }
            return imgFile;
        }
    }
}

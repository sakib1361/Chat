using Chat.Wpf.Pages.Login;
using Chat.Wpf.ViewModels;
using ChatClient.Helpers;
using ChatClient.Services;
using ChatClient.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace Chat.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize();
            ViewModelLocator.InitializeNavigation(typeof(LoginPageModel), typeof(LoginWindow));
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            //SettingService.Instance.ServerName = "serveo.net";
            //SettingService.Instance.Port = 1361;
            chatService.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Stop();
        }
    }
}

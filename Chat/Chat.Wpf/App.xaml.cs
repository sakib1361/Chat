using Chat.Wpf.Pages.Login;
using Chat.Wpf.ViewModels;
using ChatClient.Services;
using ChatClient.ViewModels;
using GalaSoft.MvvmLight.Ioc;
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
            ViewModelLocator.InitializeNavigation(typeof(LoginPageModel), typeof(LoginWindow));
            //SettingService.Instance.ServerName = "serveo.net";
            //SettingService.Instance.Port = 1361;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Stop();
        }
    }
}

using Chat.Pages.Login;
using Chat.Services;
using ChatEngine.Services;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;

namespace Chat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ViewModels.ViewModelLocator.InitializeNavigation(typeof(LoginPageModel),typeof(LoginPage));
        }

        protected override void OnStart()
        {
            string Address = SettingService.Instance.ServerName;
            int Port = SettingService.Instance.Port;
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start(Address, Port);
        }

        protected override void OnSleep()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Stop();
        }

        protected override void OnResume()
        {
            string Address = SettingService.Instance.ServerName;
            int Port = SettingService.Instance.Port;
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start(Address, Port);
            // Handle when your app resumes
        }
    }
}

using Chat.Pages.Login;
using Chat.Services;
using ChatClient.Services;
using ChatClient.ViewModels;
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

        protected override void OnSleep()
        {
            base.OnSleep();
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Resume();
        }
    }
}

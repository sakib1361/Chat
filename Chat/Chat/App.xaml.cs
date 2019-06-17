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

        protected override void OnStart()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start();
        }

        protected override void OnSleep()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Stop();
        }

        protected override void OnResume()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start();
            // Handle when your app resumes
        }
    }
}

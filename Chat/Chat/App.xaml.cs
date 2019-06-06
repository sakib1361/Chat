using Acr.UserDialogs;
using Chat.Pages.Login;
using Chat.Services;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;

namespace Chat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            NavigationHelper.Instance.Initialize(new LoginPage());
        }

        protected override void OnStart()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start("192.168.1.8", 1200);
        }

        protected override void OnSleep()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Stop();
        }

        protected override void OnResume()
        {
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start("192.168.1.8", 1200);
            // Handle when your app resumes
        }
    }
}

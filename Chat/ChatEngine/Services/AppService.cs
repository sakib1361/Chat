using ChatClient.Engine;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace ChatEngine.Services
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
    }
}

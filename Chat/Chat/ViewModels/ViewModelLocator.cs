using Chat.Pages.ChatPages;
using Chat.Pages.Home;
using Chat.Pages.Login;
using Chat.Services;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Ioc;

namespace Chat.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<ClientHandler>();
            SimpleIoc.Default.Register<ChatService>();

            SimpleIoc.Default.Register<HomePageModel>();
            SimpleIoc.Default.Register<ChatPageModel>();
            SimpleIoc.Default.Register<LoginPageModel>();
            SimpleIoc.Default.Register<RegisterPageModel>();
        }

        public HomePageModel HomePageModel => SimpleIoc.Default.GetInstance<HomePageModel>();
        public ChatPageModel ChatPageModel => SimpleIoc.Default.GetInstance<ChatPageModel>();
        public LoginPageModel LoginPageModel => SimpleIoc.Default.GetInstance<LoginPageModel>();
        public RegisterPageModel RegisterPageModel => SimpleIoc.Default.GetInstance<RegisterPageModel>();
    }
}

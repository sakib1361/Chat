using Chat.Wpf.Pages.ChatPages;
using Chat.Wpf.Pages.Login;
using Chat.Wpf.PlatformService;
using Chat.Wpf.Services;
using ChatEngine.Helpers;
using ChatEngine.Services;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows;

namespace Chat.Wpf.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
           
        }

        internal static void InitializeNavigation(Type baseView, Type type)
        {
            SimpleIoc.Default.Register<IUserDialogService, WpfDialogService>();
            SimpleIoc.Default.Register<IDispatcher, DispatcherWpf>();
            AppService.Register();
            SettingService.Instance.Init(new SettingsEngine());

            var pageModel = SimpleIoc.Default.GetInstance(baseView);
            var page = Activator.CreateInstance(type) as Window;
            var navPage = new NavigationHelper(page);
            SimpleIoc.Default.Register<INaviagationPage>(() => navPage);
            if (pageModel is BaseViewModel viewModel)
            {
                page.DataContext = viewModel;
                viewModel.OnAppearing();
            }

            navPage.Register(typeof(LoginWindow), typeof(LoginPageModel));
            navPage.Register(typeof(RegisterWindow), typeof(RegisterPageModel));
            navPage.Register(typeof(ChatView), typeof(ChatPageModel));
            navPage.Register(typeof(HomeView), typeof(HomePageModel));
            navPage.Register(typeof(ServerWindow), typeof(ServerPageModel));
        }

        public ChatPageModel ChatPageModel => SimpleIoc.Default.GetInstance<ChatPageModel>();
    }
}

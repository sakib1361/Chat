using Chat.Pages.ChatPages;
using Chat.Pages.Home;
using Chat.Pages.Login;
using Chat.PlatformService;
using Chat.Services;
using ChatClient.Engine;
using ChatEngine.Helpers;
using ChatEngine.Services;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using Xamarin.Forms;

namespace Chat.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<IUserDialogService, UserDialogService>();
            AppService.Register();
            SettingService.Instance.Init(new SettingsEngine());
        }

        internal static void InitializeNavigation(Type baseView, Type type)
        {
            var page = Activator.CreateInstance(type) as ContentPage;
            var pageModel = SimpleIoc.Default.GetInstance(baseView);
            if(pageModel is BaseViewModel viewModel)
            {
                page.BindingContext = viewModel;
                viewModel.OnAppearing();
            }

            var navHelper = new NavigationHelper(page);
            SimpleIoc.Default.Register<INaviagationPage>(() => navHelper);
            navHelper.Register(typeof(HomePage), typeof(HomePageModel));
            navHelper.Register(typeof(ChatPage), typeof(ChatPageModel));
            navHelper.Register(typeof(LoginPage), typeof(LoginPageModel));
            navHelper.Register(typeof(RegisterPage), typeof(RegisterPageModel));
            navHelper.Register(typeof(ServerPage), typeof(ServerPageModel));
        }
    }
}

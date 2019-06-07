using Chat.Wpf.PlatformService;
using ChatClient.Engine;
using ChatEngine.Services;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;

namespace Chat.Wpf.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<IUserDialogService, WpfDialogService>();
            AppService.Register();
        }

        internal static void InitializeNavigation(Type baseView, Type type)
        {

            var pageModel = SimpleIoc.Default.GetInstance<HomePageModel>();
            var page = Activator.CreateInstance(type) as MainWindow;
            if (pageModel is BaseViewModel viewModel)
            {
                page.DataContext = viewModel;
                viewModel.OnAppearing();
            }
        }
    }
}

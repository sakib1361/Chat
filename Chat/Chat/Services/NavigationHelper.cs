using Chat.ViewModels;
using ChatClient.Helpers;
using ChatClient.Services;
using ChatClient.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chat.Services
{
    public class NavigationHelper:BaseNaviagation, INaviagationPage
    {
        internal NavigationPage _navigation;

        public void Init(ContentPage contentPage)
        {
            var PrimaryDarkColor = (Color)Application.Current.Resources["primaryDarkColor"];
            _navigation = new NavigationPage(contentPage)
            {
                BarBackgroundColor = PrimaryDarkColor,
                BarTextColor = Color.White
            };
            Application.Current.MainPage = _navigation;
        }

        public void GoBack()
        {
            _navigation.PopAsync();
        }

       

        public void RemovePage()
        {
            _navigation.Navigation.RemovePage(_navigation.RootPage);
        }

        public async Task NavigateTo(Type vmType, params object[] parameter)
        {
            var pageType = GetPage(vmType);
            var page = (Page)Activator.CreateInstance(pageType);
            var viewModel = SimpleIoc.Default.GetInstance(vmType);
            if (viewModel is BaseViewModel model)
            {
                model.OnAppearing(parameter);
                page.BindingContext = model;
            }
            await _navigation.PushAsync(page,true);
        }

        public async Task NavigateBindTo(BaseViewModel baseViewModel, Type vmType, object[] args)
        {
            var pageType = GetPage(vmType);
            var page = (Page)Activator.CreateInstance(pageType);
            page.BindingContext = baseViewModel;
            await _navigation.PushAsync(page,true);
        }
    }
}

using Chat.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chat.Services
{
    public class NavigationHelper
    {
        internal NavigationPage _navigation;
        internal static NavigationHelper Instance = new NavigationHelper();
       

        public void GoBack()
        {
            _navigation.PopAsync();
        }

        public void Initialize(ContentPage contentPage)
        {
            var PrimaryDarkColor = (Color)Application.Current.Resources["primaryDarkColor"];
            _navigation = new NavigationPage(contentPage)
            {
                BarBackgroundColor = PrimaryDarkColor,
                BarTextColor = Color.White
            };
            Application.Current.MainPage = _navigation;
        }

        public void RemovePage()
        {
            _navigation.Navigation.RemovePage(_navigation.RootPage);
        }

        public async Task NavigateTo(Type pageType, params object[] parameter)
        {
            var page = (Page)Activator.CreateInstance(pageType);
            if (page.BindingContext is BaseViewModel viewModel)
                viewModel.OnAppearing(parameter);
            await _navigation.PushAsync(page,true);
           
        }

        public async Task NavigateBindTo(BaseViewModel baseViewModel, Type pageType, object[] args)
        {
            var page = (Page)Activator.CreateInstance(pageType);
            page.BindingContext = baseViewModel;
            await _navigation.PushAsync(page,true);
        }
    }

}

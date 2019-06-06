using Acr.UserDialogs;
using Chat.Pages.Home;
using Chat.PlatformService;
using Chat.Services;
using ChatClient.Engine;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Chat.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseViewModel : ViewModelBase
    {
        public bool IsBusy { get; set; }
        public bool IsRefreshing { get; set; }
        public ICommand RefreshCommand { get => new Command(RefreshAction); }
        public ICommand BackCommand { get => new Command(GoBack); }


        private void RefreshAction(object obj)
        {
            RefreshPage();
        }

        internal string Translate(string v)
        {
            return TranslationService.Instance[v];
        }

        public virtual void RefreshPage()
        {

        }

        protected void HandlerErrors(ChatObject response)
        {
            ShowMessage(response.Message);
        }

        public void ShowMessage(string message, string title = "Error")
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            UserDialogService.Instance.Message(message, title);
        }

        internal async Task<bool> ShowConfirmation(string message, string title = "Confirm")
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            return await UserDialogService.Instance.ConfirmAsync(message, title);
        }


        public string GetSettings(string key)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                var value = Application.Current.Properties[key] as string;
                return value;
            }
            else return string.Empty;
        }

        public async void SaveSetting(string key, string value)
        {
            Application.Current.Properties[key] = value;
            await Application.Current.SavePropertiesAsync();
        }


        internal void ShowToastMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            UserDialogService.Instance.Toast(message);
        }

        public virtual void OnAppearing(params object[] parameter)
        {
            IsBusy = true;
        }

        public virtual void OnDisappearing()
        {
            IsBusy = false;
        }

        public async void NavigateToPage(Type type, params object[] args)
        {
            IsBusy = true;
            await Task.Delay(50);
            await NavigationHelper.Instance.NavigateTo(type, args);
            OnDisappearing();
        }

        public async void NavigateShadowPage(Type type, params object[] args) //Bind to rootpage
        {
            IsBusy = true;
            await Task.Delay(50);
            await NavigationHelper.Instance.NavigateBindTo(this, type, args);
            OnDisappearing();
        }

        public async void MoveToPage(Type type, params object[] args)
        {
            IsBusy = true;
            await NavigationHelper.Instance.NavigateTo(type, args);
            OnDisappearing();
            NavigationHelper.Instance.RemovePage();
        }

        internal void GoBack()
        {
            if (Application.Current.MainPage is NavigationPage navPage)
                navPage.Navigation.PopAsync();
            else
            {
                var page = (Page)Activator.CreateInstance(typeof(HomePage));
                Application.Current.MainPage = new NavigationPage(page);
            }
        }
    }
}
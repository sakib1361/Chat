using Acr.UserDialogs;
using Chat.Pages.Home;
using Chat.Services;
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
        internal IUserDialogs Dialogs => UserDialogs.Instance;
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

        public async void ShowMessage(string message, string title = "Error")
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            await Dialogs.AlertAsync(message, title);
        }

        internal async Task<bool> ShowConfirmation(string message, string title = "Confirm")
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            return await Dialogs.ConfirmAsync(message, title);
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

        internal async Task<string> ShowEntryMsg(string message, bool IsCancel = true, InputType inputType = InputType.Default)
        {
            if (string.IsNullOrWhiteSpace(message)) return string.Empty;
            var result = await Dialogs.PromptAsync(new PromptConfig
            {
                Title = message,
                Text = "",
                IsCancellable = IsCancel,
                InputType = inputType
            });
            return result.Text;
        }

        internal void ShowToastMessage(string message, bool isShort = true)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var ts = TimeSpan.FromSeconds(2);
            if (isShort == false)
                ts = TimeSpan.FromSeconds(4);
            Dialogs.Toast(message, ts);
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
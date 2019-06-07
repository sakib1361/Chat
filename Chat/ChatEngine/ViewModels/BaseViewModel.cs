using ChatClient.Engine;
using ChatEngine.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatEngine.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        public bool IsBusy { get; set; }
        public bool IsRefreshing { get; set; }
        public ICommand RefreshCommand { get => new RelayCommand(RefreshAction); }
        public ICommand BackCommand { get => new RelayCommand(GoBack); }

        private void RefreshAction()
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
            var userdialog = SimpleIoc.Default.GetInstance<IUserDialogService>();
            userdialog.Message(message, title);
        }

        internal async Task<bool> ShowConfirmation(string message, string title = "Confirm")
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            var userdialog = SimpleIoc.Default.GetInstance<IUserDialogService>();
            return await userdialog.ConfirmAsync(message, title);
        }

        internal void ShowToastMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var userdialog = SimpleIoc.Default.GetInstance<IUserDialogService>();
            userdialog.Toast(message);
        }

        public virtual void OnAppearing(params object[] parameter)
        {

        }

        public virtual void OnDisappearing()
        {

        }

        public async void NavigateToPage(Type type, params object[] args)
        {
            var Naviagation = SimpleIoc.Default.GetInstance<INaviagationPage>();
            await Naviagation.NavigateTo(type, args);
            OnDisappearing();
        }

        public async void NavigateShadowPage(Type type, params object[] args) //Bind to rootpage
        {
            var Naviagation = SimpleIoc.Default.GetInstance<INaviagationPage>();
            await Naviagation.NavigateBindTo(this, type, args);
            OnDisappearing();
        }

        public async void MoveToPage(Type type, params object[] args)
        {
            var Naviagation = SimpleIoc.Default.GetInstance<INaviagationPage>();
            await Naviagation.NavigateTo(type, args);
            OnDisappearing();
            Naviagation.RemovePage();
        }

        internal void GoBack()
        {
            var Naviagation = SimpleIoc.Default.GetInstance<INaviagationPage>();
            Naviagation.GoBack();
        }
    }
}
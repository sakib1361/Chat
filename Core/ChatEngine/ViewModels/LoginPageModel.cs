using ChatCore.Engine;
using ChatClient.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class LoginPageModel : BaseViewModel
    {
        private readonly APIService _apiService;

        public string Username { get; set; }
        public string Password { get; set; }

        public LoginPageModel(APIService aPIService)
        {
            _apiService = aPIService;
#if DEBUG
            Username = "sakib51";
            Password = "pass_WORD1234";
#endif
        }

        public ICommand SignInCommand => new RelayCommand(SignInAction);
        public ICommand SignUpCommand => new RelayCommand(SignUpAction);
        public ICommand ServerCommand => new RelayCommand(ServerAction);

        private void ServerAction()
        {
            NavigateToPage(typeof(ServerPageModel));
        }

        private void SignUpAction()
        {
            NavigateToPage(typeof(RegisterPageModel));
        }

        private async void SignInAction()
        {
            if (string.IsNullOrWhiteSpace(Username))
                ShowMessage("Invalid Username");
            else if (string.IsNullOrWhiteSpace(Password))
                ShowMessage("Invalid Password");
            else
            {
                var user = new User()
                {
                    UserName = Username,
                    Password = Password
                };
                IsBusy = true;
                var res = await _apiService.Login(user);
                if (string.IsNullOrWhiteSpace(res))
                {
                    ShowMessage(WorkerService.Instance.ErrorMessage);
                }
                else
                {
                    AppService.CurrentUser = Username;
                    AppService.Token = res;
                    MoveToPage(typeof(HomePageModel));
                }
                IsBusy = false;
            }
        }
    }
}

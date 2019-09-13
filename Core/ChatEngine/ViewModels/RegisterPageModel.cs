using ChatCore.Engine;
using ChatClient.Services;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class RegisterPageModel : BaseViewModel
    {
        private readonly APIService _apiService;

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterPageModel(APIService aPIService)
        {
            _apiService = aPIService;
        }

        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            Username = string.Empty;
            Password = string.Empty;
            Firstname = Lastname = string.Empty;
#if DEBUG
            Firstname = "Shadman";
            Lastname = "Sakib";
            Username = "sakib51";
            Password = "1234";
#endif
        }
        public ICommand RegisterCommand => new RelayCommand(RegisterAction);
        public ICommand ServerCommand => new RelayCommand(ServerAction);

        private void ServerAction()
        {
            NavigateToPage(typeof(ServerPageModel));
        }

        private async void RegisterAction()
        {
            if (string.IsNullOrEmpty(Username)) ShowMessage("Invalid Username");
            else if (string.IsNullOrWhiteSpace(Password)) ShowMessage("Invaliud Password");
            else if (string.IsNullOrEmpty(Firstname)) ShowMessage("Invalid Firstname");
            else if (string.IsNullOrWhiteSpace(Lastname)) ShowMessage("Invaliud Lastname");
            else
            {
                var user = new User()
                {
                    UserName = Username,
                    Password = Password,
                    FirstName = Firstname,
                    LastName = Lastname
                };
                IsBusy = true;
                var res = await _apiService.Register(user);
                if (string.IsNullOrEmpty(res))
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

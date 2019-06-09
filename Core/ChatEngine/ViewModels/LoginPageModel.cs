using ChatClient.Engine;
using ChatEngine.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Windows.Input;

namespace ChatEngine.ViewModels
{
    public class LoginPageModel : BaseViewModel
    {
        private readonly ChatService ChatService;
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginPageModel(ChatService chatService)
        {
            ChatService = chatService;
#if DEBUG
            Username = "sakib51";
            Password = "1234";
#endif
        }

        private void LoginSuccess(ChatObject obj)
        {
            AppService.CurrentUser = Username;
            MoveToPage(typeof(HomePageModel));
        }

        private void LoginFailed(ChatObject obj)
        {
            IsBusy = false;
            ShowMessage(obj.Message);
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
                    Username = Username,
                    Password = Password
                };
                var chat = new ChatObject(MessageType.Subscribe)
                {
                    SenderName = Username,
                    Message = JsonConvert.SerializeObject(user)
                };
                AppService.CurrentUser = Username;
                IsBusy = true;
                var res = await ChatService.GetData(chat);
                if (res != null)
                {
                    if (res.MessageType == MessageType.Failed) HandlerErrors(res);
                    else if (res.MessageType == MessageType.LoginFailed) LoginFailed(res);
                    else if (res.MessageType == MessageType.LoginSuccess) LoginSuccess(res);
                    else ShowMessage(res.Message);
                }
                IsBusy = false;
            }
        }
    }
}

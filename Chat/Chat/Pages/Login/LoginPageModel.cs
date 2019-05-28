using Chat.Pages.Home;
using Chat.Services;
using Chat.ViewModels;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.Pages.Login
{
    public class LoginPageModel : BaseViewModel
    {
        private readonly ChatService ChatService;
        public string Username { get; set; }
        public string Password { get; set; }


        public LoginPageModel(ChatService chatService)
        {
            Messenger.Default.Register<ChatObject>(this, MessageType.LoginFailed, Login_Failed);
            Messenger.Default.Register<ChatObject>(this, MessageType.LoginSuccess, Login_Success);
            this.ChatService = chatService;
#if DEBUG
            Username = "sakib51";
            Password = "1234";
#endif
        }

        private void Login_Success(ChatObject obj)
        {
            AppService.CurrentUser = Username;
            MoveToPage(typeof(HomePage));
        }

        private void Login_Failed(ChatObject obj)
        {
            IsBusy = false;
            ShowMessage(obj.Message);
        }

        public ICommand SignInCommand => new RelayCommand(SignInAction);
        public ICommand SignUpCommand => new RelayCommand(SignUpAction);

        private void SignUpAction()
        {
            NavigateToPage(typeof(RegisterPage));
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
                ChatService.SendMessage(new ChatObject(MessageType.Subscribe)
                {
                    Message = JsonConvert.SerializeObject(user)
                });
                IsBusy = true;
                await Task.Delay(5000);
                IsBusy = false;
            }
        }
    }
}

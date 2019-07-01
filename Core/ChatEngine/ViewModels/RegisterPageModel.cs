using ChatCore.Engine;
using ChatClient.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Windows.Input;
using System.Diagnostics;
using System;

namespace ChatClient.ViewModels
{
    public class RegisterPageModel : BaseViewModel
    {
        private readonly ChatService chatService;
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterPageModel(ChatService chatService)
        {
            this.chatService = chatService;
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
                    Username = Username,
                    Password = Password,
                    Firstname = Firstname,
                    Lastname = Lastname
                };
                var chat = new ChatObject(MessageType.Register)
                {
                    SenderName = Username,
                    Message = JsonConvert.SerializeObject(user)
                };
                AppService.CurrentUser = Username;
                IsBusy = true;
                var res = await chatService.GetData(chat);
                if (res != null)
                {
                    if (res.MessageType == MessageType.Failed) HandlerErrors(res);
                    else if (res.MessageType == MessageType.LoginFailed) Registar_Failed(res);
                    else if (res.MessageType == MessageType.LoginSuccess) Login_Success(res);
                    else ShowMessage(res.Message);
                }
                IsBusy = false;
            }
        }
        private void Login_Success(ChatObject obj)
        {
            AppService.CurrentUser = Username;
            MoveToPage(typeof(HomePageModel));
            Debug.WriteLine(obj.Message);
        }

        private void Registar_Failed(ChatObject obj)
        {
            ShowMessage(obj.Message);
        }

    }
}

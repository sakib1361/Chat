using Chat.Helpers;
using Chat.Pages.Home;
using Chat.Services;
using Chat.ViewModels;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Input;

namespace Chat.Pages.Login
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
            Messenger.Default.Register<ChatObject>(this, MessageType.RegistrationFailed, Registar_Failed);
            Messenger.Default.Register<ChatObject>(this, MessageType.LoginSuccess, Login_Success);
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

        private void Login_Success(ChatObject obj)
        {
            MoveToPage(typeof(HomePage));
            AppService.CurrentUser = Username;
        }

        private void Registar_Failed(ChatObject obj)
        {
            ShowMessage(obj.Message);
        }

        public ICommand RegisterCommand => new RelayCommand(RegisterAction);

        private void RegisterAction()
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
                    Message = JsonConvert.SerializeObject(user)
                };
                chatService.SendMessage(chat);
            }
        }
    }
}

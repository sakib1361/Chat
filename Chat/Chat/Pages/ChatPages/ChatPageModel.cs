using Chat.Services;
using Chat.ViewModels;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Chat.Pages.ChatPages
{
    public class ChatPageModel : BaseViewModel
    {
        private readonly ChatService chatService;
        public string Receiver { get; private set; }
        public string Message { get; set; }
        public ObservableCollection<ChatObject> ChatObjects { get; private set; }
        public ChatPageModel(ChatService chatService)
        {
            this.chatService = chatService;
            ChatObjects = new ObservableCollection<ChatObject>();
            Messenger.Default.Register<ChatObject>(this, MessageType.EndToEnd, NewMessage);
        }

        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            ChatObjects.Clear();
            if (parameter.Length > 0 && parameter[0] is string username)
            {
                Receiver = username;
                LoadHistory();
            }
        }

        private async void LoadHistory()
        {
            var msg = new ChatObject(MessageType.GetHistory)
            {
                SenderName = AppService.CurrentUser,
                ReceiverName = Receiver
            };
            var response = await chatService.GetData(msg);
            if (response.MessageType == MessageType.Failed) HandlerErrors(response);
            else
            {
                var oldChats = JsonConvert.DeserializeObject<List<ChatObject>>(response.Message);
                foreach (var item in oldChats)
                {
                    if (ChatObjects.Any(x => x.Id == item.Id) == false)
                        ChatObjects.Add(item);
                }
            }
        }



        private void NewMessage(ChatObject obj)
        {
            if (obj.SenderName == Receiver)
            {
                if (ChatObjects.Any(x => x.Id == obj.Id) == false)
                {
                    ChatObjects.Add(obj);
                }
            }
        }

        public ICommand SendCommand => new RelayCommand(SendAction);

        private async void SendAction()
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                var chat = new ChatObject(MessageType.EndToEnd)
                {
                    SenderName = AppService.CurrentUser,
                    ReceiverName = Receiver,
                    Message = Message
                };
                if (await chatService.SendMessage(chat))
                {
                    ChatObjects.Add(chat);
                }
                Message = string.Empty;
            }
        }
    }
}

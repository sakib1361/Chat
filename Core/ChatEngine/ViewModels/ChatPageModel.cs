using ChatClient.Engine;
using ChatEngine.Helpers;
using ChatEngine.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ChatEngine.ViewModels
{
    public class ChatPageModel : BaseViewModel
    {
        private readonly IDispatcher Dispatcher;
        private readonly ChatService chatService;
        public string Receiver { get; private set; }
        public string Message { get; set; }
        public ObservableCollection<ChatObject> ChatObjects { get; private set; }
        public ChatPageModel(ChatService chatService, IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
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
                        ChatObjects.Insert(0, item);
                }
                if (oldChats.Count > 0)
                {
                    Messenger.Default.Send(AppConstants.NewMessage, AppConstants.NewMessage);
                }
            }
        }

        private void NewMessage(ChatObject obj)
        {
            if (obj.SenderName == Receiver)
            {
                Dispatcher.RunAsync(() =>
                {
                    ChatObjects.Add(obj);
                    Messenger.Default.Send(AppConstants.NewMessage, AppConstants.NewMessage);
                });
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
                    Messenger.Default.Send(AppConstants.NewMessage, AppConstants.NewMessage);
                }
                Message = string.Empty;
            }
        }
    }
}

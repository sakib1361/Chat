using Chat.Services;
using Chat.ViewModels;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            Messenger.Default.Register<ChatObject>(this, MessageType.GetHistory, HistoryMessage);
        }

        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            if (parameter.Length > 0 && parameter[0] is string username)
            {
                Receiver = username;
                chatService.SendMessage(new ChatObject(MessageType.GetHistory)
                {
                    SenderName = AppService.CurrentUser,
                    ReceiverName = Receiver
                });
            }
        }

        private void HistoryMessage(ChatObject obj)
        {
            try
            {
                if (obj.SenderName == Receiver)
                {
                    var oldChats = JsonConvert.DeserializeObject<List<ChatObject>>(obj.Message);
                    foreach (var item in oldChats)
                    {
                        if (ChatObjects.Any(x => x.Id == item.Id) == false)
                        {
                            ChatObjects.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
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

        private void SendAction()
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                var chat = new ChatObject(MessageType.EndToEnd)
                {
                    SenderName = AppService.CurrentUser,
                    ReceiverName = Receiver,
                    Message = Message
                };
                chatService.SendMessage(chat);
            }
        }
    }
}

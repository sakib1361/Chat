using ChatCore.Engine;
using ChatClient.Helpers;
using ChatClient.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class ChatPageModel : BaseViewModel
    {
        private readonly IDispatcher Dispatcher;
        private readonly ChatService _chatService;
        private readonly APIService _apiService;

        public string Receiver { get; private set; }
        public string Message { get; set; }
        public ObservableCollection<ChatObject> ChatObjects { get; private set; }
        public ChatPageModel(ChatService chatService,APIService aPIService, IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            _chatService = chatService;
            _apiService = aPIService;
            
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
            var response = await _apiService.GetHistory(Receiver);
            if (response == null)
            {
                ShowMessage(WorkerService.Instance.ErrorMessage);
            }
            else
            {
                foreach (var item in response)
                {
                    if (ChatObjects.Any(x => x.Id == item.Id) == false)
                        ChatObjects.Insert(0, item);
                }
                if (response.Count > 0)
                {
                    Messenger.Default.Send(AppConstants.NewMessage, AppConstants.NewMessage);
                }
            }
        }

        private void NewMessage(ChatObject obj)
        {
            if (obj.SenderName == Receiver || 
                (obj.SenderName == AppService.CurrentUser && obj.ReceiverName == Receiver))
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
                if (await _chatService.SendMessage(chat))
                {
                    ChatObjects.Add(chat);
                    Messenger.Default.Send(AppConstants.NewMessage, AppConstants.NewMessage);
                }
                Message = string.Empty;
            }
        }
    }
}

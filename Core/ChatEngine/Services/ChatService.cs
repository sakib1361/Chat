using ChatCore.Engine;
using ChatClient.Helpers;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChatClient.Services
{
    public class ChatService
    {
        private readonly ClientHandler ClientHandler;
        private readonly IUserDialogService DialogService;
        public ChatService(ClientHandler clientHandler, IUserDialogService dialogService)
        {
            ClientHandler = clientHandler;
            DialogService = dialogService;
            ClientHandler.MessageRecieveed += ClientHandler_MessageRecieveed;
            ClientHandler.ConnectionChanged += ClientHandler_ClientStateChanged;
        }

        private void ClientHandler_ClientStateChanged(object sender, string e)
        {
            DialogService.Toast(e);
        }

        private void ClientHandler_MessageRecieveed(object sender, ChatObject e)
        {
            if (e.MessageType == MessageType.EndToEnd || e.MessageType == MessageType.BroadCast)
                Messenger.Default.Send(e, e.MessageType);
        }

        public void Stop()
        {
            ClientHandler?.Close();
        }

        public async Task<bool> SendMessage(ChatObject chatObject)
        {
            try
            {
                await ClientHandler.SendMessage(chatObject);
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return false;
            }
        }

        public async void Start()
        {
            Stop();
            await Task.Delay(3000);
            var address = SettingService.Instance.ServerName;
            var port = SettingService.Instance.Port;
            ClientHandler.Connect(address, port);
        }
    }
}

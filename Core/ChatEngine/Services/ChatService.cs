using ChatCore.Engine;
using ChatClient.Helpers;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Diagnostics;

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

        private async void ClientHandler_ClientStateChanged(object sender, WebSocketState e)
        {
            DialogService.Toast("Client State Changed" + e.ToString());
            if (e == WebSocketState.Open)
            {
                await Task.Delay(2000);
                var res = await SendMessage(new ChatObject(MessageType.Subscribe)
                {
                    Message = AppService.Token,
                    SenderName = AppService.CurrentUser
                });
                Debug.WriteLine(res);
            }
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

        public void Start()
        {
            Stop();
            var address = SettingService.Instance.ServerName;
            var port = SettingService.Instance.Port;
            var ssl = SettingService.Instance.AllowSSL;
            ClientHandler.Connect(address, port, ssl);
        }
    }
}

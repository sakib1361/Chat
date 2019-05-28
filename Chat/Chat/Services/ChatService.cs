using Acr.UserDialogs;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chat.Services
{
    public class ChatService
    {
        private readonly ClientHandler ClientHandler;
        public ChatService(ClientHandler clientHandler)
        {
            ClientHandler = clientHandler;
            ClientHandler.MessageRecieveed += ClientHandler_MessageRecieveed;
            ClientHandler.ConnectionChanged += ClientHandler_ClientStateChanged;
        }

        private void ClientHandler_ClientStateChanged(object sender, string e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.Toast(e);
            });
        }

        private void ClientHandler_MessageRecieveed(object sender, ChatObject e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Messenger.Default.Send(e, e.MessageType);
            });
        }

        internal void Stop()
        {
            ClientHandler?.Close();
        }

        public async void SendMessage(ChatObject chatObject)
        {
            try
            {
                await ClientHandler.SendMessage(chatObject);
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

        public void Start(string address, int port)
        {
            ClientHandler.Connect(address, port);
        }
    }
}

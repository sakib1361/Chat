using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatCore.Engine
{
    public class ClientHandler
    {
        SocketHandler SocketHandler;

        private string Address;
        private static bool Quit;
        private static bool IsConnecting;
        private readonly Random Random = new Random();

        public event EventHandler<ChatObject> MessageRecieveed;
        public event EventHandler<WebSocketState> ConnectionChanged;

        public void Connect(string address, int port, bool ssl, bool allowPort)
        {
            var socket = ssl ? "wss" : "ws";

            if (allowPort)
            {
                Address = string.Format("{0}://{1}:{2}/home/getSocket", socket, address, port);
            }
            else
            {
                Address = string.Format("{0}://{1}/home/getSocket", socket, address);
            }

            Quit = false;
            StartConnectionService();
        }

        private async void StartConnectionService()
        {
            if (IsConnecting) return;
            IsConnecting = true;
            Quit = false;
            await Task.Factory.StartNew(async () =>
            {
                bool res = SocketHandler != null && SocketHandler.IsActive;
                while (!res && !Quit)
                {
                    res = await TryConnect();
                    if (res) ConnectionChanged?.Invoke(this, WebSocketState.Open);
                    else await Task.Delay(10000);
                }
                IsConnecting = false;
            });
        }

        private async Task<bool> TryConnect()
        {
            try
            {
                var tcp = new ClientWebSocket();
                await tcp.ConnectAsync(new Uri(Address),new CancellationToken());
                DisposeSocket();
                SocketHandler = new SocketHandler(tcp);
                SocketHandler.MessageReceived += SocketHandler_MessageReceived;
                SocketHandler.ClientDisconnected += SocketHandler_ClientDisconnected;
                Recieve();
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return false;
            }
        }

        private async void Recieve()
        {
            await SocketHandler.StartReceive();
        }

        public async Task SendMessage(ChatObject chatObject)
        {
            if(!IsConnecting)
            await SocketHandler?.SendMessage(chatObject);
        }

        private async void SocketHandler_ClientDisconnected(object sender, string e)
        {
            SocketHandler?.Dispose();
            if (!Quit)
            {
                ConnectionChanged?.Invoke(this, WebSocketState.Closed);
                var randomeDelay = Random.Next(2000, 10000);
                await Task.Delay(randomeDelay);
                StartConnectionService();
            }
        }

        private void SocketHandler_MessageReceived(object sender, ChatObject e)
        {
            MessageRecieveed?.Invoke(sender, e);
        }

        public void Close()
        {
            Quit = true;
            DisposeSocket();
            IsConnecting = false;
        }

        private void DisposeSocket()
        {
            try
            {
                SocketHandler?.Dispose();
                SocketHandler.MessageReceived -= SocketHandler_MessageReceived;
                SocketHandler.ClientDisconnected -= SocketHandler_ClientDisconnected;
                SocketHandler = null;
            }
            catch { }
        }
    }
}

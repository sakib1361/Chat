using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient.Engine
{
    public class ClientHandler
    {
        SocketHandler SocketHandler;

        private string Address;
        private int Port;
        private static bool Quit;
        private static bool IsConnecting;
        private Random Random = new Random();

        public event EventHandler<ChatObject> MessageRecieveed;
        public event EventHandler<string> ConnectionChanged;

        public void Connect(string address, int port)
        {
            Address = address;
            Port = port;
            Quit = false;
            StartConnectionService();
        }

        private async void StartConnectionService()
        {
            if (IsConnecting) return;
            IsConnecting = true;
            await Task.Factory.StartNew(async () =>
            {
                bool res = SocketHandler != null && SocketHandler.IsActive;
                while (!res && !Quit)
                {
                    res = await TryConnect();
                    if (res) ConnectionChanged?.Invoke(this, "Client Connected");
                    else await Task.Delay(10000);
                }
                IsConnecting = false;
            });
        }

        private async Task<bool> TryConnect()
        {
            try
            {
                var tcp = new TcpClient();
                await tcp.ConnectAsync(Address, Port);
                DisposeSocket();
                SocketHandler = new SocketHandler(tcp);
                SocketHandler.MessageReceived += SocketHandler_MessageReceived;
                SocketHandler.ClientDisconnected += SocketHandler_ClientDisconnected;
                SocketHandler.StartReceive();
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return false;
            }
        }

        public async Task SendMessage(ChatObject chatObject)
        {
            await SocketHandler?.SendMessage(chatObject);
        }

        private async void SocketHandler_ClientDisconnected(object sender, string e)
        {
            SocketHandler?.Dispose();
            if (!Quit)
            {
                ConnectionChanged?.Invoke(this, "Server Diconnected");
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

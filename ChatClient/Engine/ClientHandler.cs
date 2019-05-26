using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient.Engine
{
    public class ClientHandler
    {
        SocketHandler SocketHandler;

        private string Address;
        private int Port;
        private bool Quit;

        public event EventHandler<ChatObject> MessageRecieveed;
        public event EventHandler<string> ClientDisconnected;

        public async Task<bool> Connect(string address, int port)
        {
            Address = address;
            Port = port;
            try
            {
                var tcp = new TcpClient();
                await tcp.ConnectAsync(address, port);
                SocketHandler = new SocketHandler(tcp);
                SocketHandler.MessageReceived += SocketHandler_MessageReceived;
                SocketHandler.ClientDisconnected += SocketHandler_ClientDisconnected;
                SocketHandler.StartReceive();
                return false;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return false;
            }
        }

        public async Task<bool> SendMessage(ChatObject chatObject)
        {
            try
            {
                await SocketHandler.SendMessage(chatObject);
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return false;
            }
        }

        private async void SocketHandler_ClientDisconnected(object sender, string e)
        {
            SocketHandler?.Dispose();
            if (!Quit)
            {
                ClientDisconnected?.Invoke(this, e);
                await Connect(Address, Port);
            }
        }

        private void SocketHandler_MessageReceived(object sender, ChatObject e)
        {
            MessageRecieveed?.Invoke(sender, e);
        }

        public void Close()
        {
            Quit = true;
            SocketHandler?.Dispose();
        }
    }
}

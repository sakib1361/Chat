using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ChatClient.Engine
{
    public class SocketHandler : IDisposable
    {
        private readonly TcpClient TcpClient;
        public event EventHandler<ChatObject> MessageReceived;
        public event EventHandler<string> ClientDisconnected;
        public bool IsActive { get; private set; }
        public SocketHandler(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public async Task SendMessage(ChatObject chatObject)
        {
            var datastream = TcpClient.GetStream();
            var formatter = new BinaryFormatter();
            await Task.Run(() => formatter.Serialize(datastream, chatObject));
        }

        public async void StartReceive()
        {
            await Task.Run(() =>
            {
                var formatter = new BinaryFormatter();
                var datastream = TcpClient.GetStream();
                while (TcpClient.Connected)
                {
                    IsActive = true;
                    try
                    {
                        var message = (ChatObject)formatter.Deserialize(datastream);
                        MessageReceived?.Invoke(this, message);
                    }
                    catch (Exception ex)
                    {
                        LogEngine.Error(ex);
                        break;
                    }
                }
                IsActive = false;
                ClientDisconnected?.Invoke(this, "General Disconnect");
            });
        }

        public void Dispose()
        {
            TcpClient?.Close();
            TcpClient?.Dispose();
        }
    }
}

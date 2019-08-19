using System;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatCore.Engine
{
    public class SocketHandler : IDisposable
    {
        private readonly WebSocket WebSocket;
        private readonly CancellationToken CToken;

        public event EventHandler<ChatObject> MessageReceived;
        public event EventHandler<string> ClientDisconnected;

        readonly BinaryFormatter bf = new BinaryFormatter();
        public bool IsActive { get; private set; }
        public SocketHandler(WebSocket webSocket)
        {
            WebSocket = webSocket;
            CToken = new CancellationToken();
        }

        private byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private ChatObject DeSerialize(Stream obj)
        {
            return (ChatObject)bf.Deserialize(obj);
        }

        public async Task SendMessage(ChatObject chatObject)
        {
            var data = Serialize(chatObject);
            await WebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CToken);
        }

        public async void StartReceive()
        {
            while (WebSocket.State == WebSocketState.Open)
            {
                IsActive = true;
                try
                {
                    var message = await GetMessage();
                    MessageReceived?.Invoke(this, message);
                }
                catch (Exception ex)
                {
                    LogEngine.Error(ex);
                    break;
                }
                finally
                {
                    ClientDisconnected?.Invoke(this,"Socket Disconnected");
                }
            }
        }

        private async Task<ChatObject> GetMessage()
        {
            var buffer = new ArraySegment<byte>(new byte[10240]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    return DeSerialize(ms);
                }
                else return null;
            }
        }

        public void Dispose()
        {
            WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,"Socket close",CToken);
            WebSocket.Dispose();
        }
    }
}

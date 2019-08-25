using Newtonsoft.Json;
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
        public bool IsActive { get; private set; }
        public SocketHandler(WebSocket webSocket)
        {
            WebSocket = webSocket;
            CToken = new CancellationToken();
        }

        public async Task SendMessage(ChatObject chatObject)
        {
            var serialized = JsonConvert.SerializeObject(chatObject);
            var data = Encoding.UTF8.GetBytes(serialized);
            await WebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CToken);
        }

        public async Task StartReceive()
        {
            while (WebSocket.State == WebSocketState.Open)
            {
                IsActive = true;
                try
                {
                    var message = await GetMessage();
                    if (message != null)
                        MessageReceived?.Invoke(this, message);
                }
                catch (Exception ex)
                {
                    LogEngine.Error(ex);
                    ClientDisconnected?.Invoke(this, "Socket Disconnected");
                    break;
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

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var deserialized = Encoding.UTF8.GetString(ms.ToArray());
                    return JsonConvert.DeserializeObject<ChatObject>(deserialized);
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

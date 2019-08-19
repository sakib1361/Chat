using ChatCore.Engine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ChatServer.Engine.Network
{
    public class ServerHandler
    {
        private bool Quit;
        private readonly MessageHandler MessageHandler;
        private readonly List<SocketHandler> sockets;
        public ServerHandler(MessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
            sockets = new List<SocketHandler>();
        }

        public async Task Start(WebSocket webSocket)
        {
            if (Quit) return;
            var socket = new SocketHandler(webSocket);

            socket.MessageReceived += Socket_MessageReceived;
            socket.ClientDisconnected += Socket_ClientDisconnected;
            sockets.Add(socket);
            Console.WriteLine("Socket Cnnected ");
            await socket.StartReceive();
        }

        public void Close()
        {
            Quit = true;
            foreach (var item in sockets) item.Dispose();
            sockets.Clear();
        }

        private void Socket_ClientDisconnected(object sender, string e)
        {
            Console.WriteLine(e);
            if(sender is SocketHandler socketHandler)
            {
                sockets.Remove(socketHandler);
                socketHandler.Dispose();
                MessageHandler.BroadcastLogout(socketHandler);
            }
        }

        private void Socket_MessageReceived(object sender, ChatObject e)
        {
            if (sender is SocketHandler socketHandler)
            {
                try
                {
                    MessageHandler.MessageRecieved(socketHandler, e);
                }
                catch(Exception ex)
                {
                    LogEngine.Error(ex);
                }
            }
        }
    }
}

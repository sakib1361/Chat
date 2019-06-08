using ChatClient.Engine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer.Engine.Network
{
    public class ServerHandler
    {
        readonly TcpListener TcpListener;
        private bool Quit;
        private readonly MessageHandler MessageHandler;
        private readonly List<SocketHandler> sockets;
        public ServerHandler(MessageHandler messageHandler, int port)
        {
            MessageHandler = messageHandler;
            sockets = new List<SocketHandler>();
            TcpListener = new TcpListener(IPAddress.Any, port);
            
        }

        public async void Start()
        {
            await Task.Run(async () =>
            {
                TcpListener.Start();
                while (!Quit)
                {
                    var tcp = await TcpListener.AcceptTcpClientAsync();
                    var socket = new SocketHandler(tcp);
                    socket.StartReceive();
                    socket.MessageReceived += Socket_MessageReceived;
                    socket.ClientDisconnected += Socket_ClientDisconnected;
                    sockets.Add(socket);
                    Console.WriteLine("Socket Cnnected ");
                }
            });
        }

        public void Close()
        {
            Quit = true;
            TcpListener.Stop();
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

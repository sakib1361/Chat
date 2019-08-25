using ChatCore.Engine;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ServerTest
{
    class TestChatService
    {
        private readonly string Username;
        private readonly ClientHandler ClientHandler;
        public TestChatService(ClientHandler clientHandler, string username)
        {
            Username = username;
            ClientHandler = clientHandler;
            ClientHandler.MessageRecieveed += ClientHandler_MessageRecieveed;
            ClientHandler.ConnectionChanged += ClientHandler_ClientStateChanged;
        }

        private void ClientHandler_ClientStateChanged(object sender, string e)
        {
            Console.WriteLine(e);
        }

        private void ClientHandler_MessageRecieveed(object sender, ChatObject e)
        {
            if (e.MessageType == MessageType.EndToEnd || e.MessageType == MessageType.BroadCast)
                Console.WriteLine(e.ToString());
        }

        internal void Stop()
        {
            ClientHandler?.Close();
        }

        public async Task SendMessage(ChatObject chatObject)
        {
            try
            {
                await ClientHandler.SendMessage(chatObject);
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
            }
        }


        public void Start(string address, int port)
        {
            ClientHandler.Connect(address, port);
        }
    }
}

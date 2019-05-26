using ChatClient.Engine;
using ChatServer.Engine.Database;
using ChatServer.Engine.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            StartAsync();
            LogEngine.ErrorOccured += (s, e) => Console.WriteLine(e);
            Console.ReadLine();
        }

        private static async void StartAsync()
        {
            var db = new DBHandler();
            var messageHandler = new MessageHandler(db);
            var server = new ServerHandler(messageHandler, 1200);
            server.Start();
            await Task.Delay(1000);

            var client = new ClientHandler();
            await client.Connect("127.0.0.1", 1200);


            client.MessageRecieveed += (s, e) =>
            {
                Console.WriteLine(e.ToString());
            };
            var user = new User()
            {
                Username = "sakib",
                Password = "1234",
            };
            var chatMessage = new ChatObject()
            {
                MessageType = MessageType.Register,
                Message = JsonConvert.SerializeObject(user)
            };
            await client.SendMessage(chatMessage);
        }
    }
}

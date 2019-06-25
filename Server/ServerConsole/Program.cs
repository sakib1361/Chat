using ChatCore.Engine;
using ChatServer.Engine.Database;
using ChatServer.Engine.Network;
using System;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            LogEngine.ErrorOccured += (s, e) => Console.WriteLine(e);
            Start();
            Console.ReadLine();
        }

        private static void Start()
        {
            var db = new DBHandler();
            var messageHandler = new MessageHandler(db);
            var server = new ServerHandler(messageHandler, 1200);
            server.Start();
            Console.WriteLine("Server Started");
        }
    }
}

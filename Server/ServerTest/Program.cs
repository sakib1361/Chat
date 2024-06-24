using ChatCore.Engine;
using System;
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
            await Task.Delay(1000);

            var client = new ClientHandler();

            var username = Guid.NewGuid().ToString();
            var testService = new TestChatService(client, username);
            testService.Start("localhost", 50088);
            await Task.Delay(5000);
           
        }
    }
}

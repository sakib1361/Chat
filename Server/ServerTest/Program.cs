using ChatCore.Engine;
using Newtonsoft.Json;
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
            var res = await testService.GetData(new ChatObject()
            {
                MessageType = MessageType.Register,
                SenderName = username,
                ReceiverName = "Server",
                Message = JsonConvert.SerializeObject(new User()
                {
                    Username = username,
                    Password = "1234"
                })
            });
       
            var allUsers = await testService.GetData(new ChatObject()
            {
                SenderName = username,
                MessageType = MessageType.GetUsers,
            });
            Console.WriteLine(allUsers);
            
        }
    }
}

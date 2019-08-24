using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatCore.Engine;
using ChatServer.Engine.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChatServer.Engine.Network
{
    public class MessageHandler
    {
        private readonly LocalDBContext _localDB;

        // string being userId with a list of socket instances
        private readonly Dictionary<string, List<SocketHandler>> AllSocketInstances;
        public MessageHandler(LocalDBContext localDB)
        {
            _localDB = localDB;
            AllSocketInstances = new Dictionary<string, List<SocketHandler>>();
        }
        internal void BroadcastLogout(SocketHandler socketHandler)
        {
            var res = AllSocketInstances.FirstOrDefault(x => x.Value.Contains(socketHandler));
            if (res.Value != null)
            {
                foreach (var socket in res.Value.ToList())
                {
                    if (!socket.IsActive) res.Value.Remove(socket);
                }
                if (res.Value.Count == 0)
                {// User has been logged out from all instances
                    AllSocketInstances.Remove(res.Key);
                    Console.WriteLine("Logged out instance " + res.Key);
                }
            }
        }

        internal void MessageRecieved(SocketHandler socketHandler, ChatObject e)
        {
            Console.WriteLine(e.MessageType + " => " + e.Message);
            switch (e.MessageType)
            {
                case MessageType.EndToEnd:
                    SendEndToEndMessage(e);
                    break;
                case MessageType.Subscribe:
                    SubscribeUser(socketHandler, e);
                    break;
            }
        }

        public async Task<List<IDUser>> GetUsers()
        {
            var users = await _localDB.Users.OfType<IDUser>().ToListAsync();
            foreach (var user in users)
            {
                user.Active = AllSocketInstances.ContainsKey(user.UserName);
            }
            users = users.OrderByDescending(x => x.Active).ToList();
            return users;
        }

        private void SubscribeUser(SocketHandler socketHandler, ChatObject e)
        {
            var user = JsonConvert.DeserializeObject<User>(e.Message);
            var dbUser = _localDB.Users.OfType<IDUser>()
                                 .FirstOrDefault(x => x.UserName == user.Username);

            if (AllSocketInstances.ContainsKey(dbUser.UserName))
            {
                AllSocketInstances[dbUser.UserName].Add(socketHandler);
            }
            else
            {
                AllSocketInstances[dbUser.UserName] = new List<SocketHandler> { socketHandler };
            }
            SendServerResponse(socketHandler, MessageType.LoginSuccess, e);
        }



        private async void SendServerResponse(SocketHandler socketHandler,
            MessageType messageType,
            ChatObject original,
            string message = "Server Message")
        {
            try
            {
                await socketHandler.SendMessage(new ChatObject()
                {
                    ChatId = original.ChatId,
                    SenderName = "Server",
                    ReceiverName = original.SenderName,
                    MessageType = messageType,
                    Message = message
                });
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
            }
        }



        private async void SendEndToEndMessage(ChatObject e)
        {
            if (AllSocketInstances.ContainsKey(e.ReceiverName))
            {
                var rInstances = AllSocketInstances[e.ReceiverName];
                if (rInstances.Count > 0)
                {
                    foreach (var item in rInstances)
                    {
                        try
                        {
                            await item.SendMessage(e);
                            e.Delivered = true;
                        }
                        catch (Exception ex)
                        {
                            LogEngine.Error(ex);
                        }
                    }
                }

            }

            _localDB.Add(e);
            await _localDB.SaveChangesAsync();
        }
    }
}

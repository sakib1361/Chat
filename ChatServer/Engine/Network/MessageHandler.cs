using System;
using System.Collections.Generic;
using System.Linq;
using ChatClient.Engine;
using ChatServer.Engine.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChatServer.Engine.Network
{
    public class MessageHandler
    {
        private readonly DBHandler DBHandler;
        // int being userId with a list of socket instances
        private readonly Dictionary<string, List<SocketHandler>> AllSocketInstances;
        public MessageHandler(DBHandler dBHandler)
        {
            DBHandler = dBHandler;
            AllSocketInstances = new Dictionary<string, List<SocketHandler>>();
        }
        internal void BroadcastLogout(SocketHandler socketHandler)
        {
            var res = AllSocketInstances.FirstOrDefault(x => x.Value.Contains(socketHandler));
            if (res.Value != null && res.Value.Count == 0)
            {
                // User has been logged out from all instances
                AllSocketInstances.Remove(res.Key);
            }
        }

        internal void MessageRecieved(SocketHandler socketHandler, ChatObject e)
        {
            switch (e.MessageType)
            {
                case MessageType.EndToEnd:
                    SendEndToEndMessage(e);
                    break;
                case MessageType.Register:
                    RegisterUser(socketHandler, e);
                    break;
                case MessageType.Subscribe:
                    SubscribeUser(socketHandler, e);
                    break;
                case MessageType.GetHistory:
                    GetUserHistory(socketHandler, e);
                    break;
                case MessageType.GetUsers:
                    GetUsers(socketHandler, e);
                    break;
            }
        }

        private async void GetUsers(SocketHandler socketHandler, ChatObject e)
        {
            if (AllSocketInstances[e.SenderName] != null)
            {
                using (var db = DBHandler.Create())
                {
                    var users = await db.Users.ToListAsync();
                    foreach (var user in users)
                    {
                        user.Active = AllSocketInstances[user.Username] != null;
                    }
                    users = users.OrderByDescending(x => x.Active).ToList();
                    var fulMessage = JsonConvert.SerializeObject(users);
                    SendServerResponse(socketHandler, MessageType.GetHistory, fulMessage);
                }
            }
        }

        private async void GetUserHistory(SocketHandler socketHandler, ChatObject e)
        {
            using (var db = DBHandler.Create())
            {
                var skip = 0;
                if (!string.IsNullOrWhiteSpace(e.Message) && int.TryParse(e.Message, out int res))
                {
                    skip = res;
                }

                var messages = await db.ChatObjects
                                 .Where(x => (x.SenderName == e.SenderName ||
                                            x.SenderName == e.ReceiverName)
                                     && (x.ReceiverName == x.ReceiverName ||
                                        x.ReceiverName == x.SenderName))
                                 .OrderByDescending(m => m.CreatedOn)
                                 .Skip(skip).Take(30)
                                 .ToListAsync();
                var fulMessage = JsonConvert.SerializeObject(messages);
                SendServerResponse(socketHandler, MessageType.GetHistory, fulMessage);
            }
        }

        private void SubscribeUser(SocketHandler socketHandler, ChatObject e)
        {
            var user = JsonConvert.DeserializeObject<User>(e.Message);
            using (var db = DBHandler.Create())
            {
                var dbUser = db.Users.FirstOrDefault(x => x.Username == user.Username);
                if (dbUser.StoredPassword == user.Password)
                {
                    if (AllSocketInstances[dbUser.Username] == null)
                    {
                        AllSocketInstances[dbUser.Username] = new List<SocketHandler> { socketHandler };
                    }
                    else
                    {
                        AllSocketInstances[dbUser.Username].Add(socketHandler);
                    }
                    SendServerResponse(socketHandler, MessageType.LoginSuccess);
                }
                else SendServerResponse(socketHandler, MessageType.LoginFailed);
            }
        }
        private async void RegisterUser(SocketHandler socketHandler, ChatObject e)
        {
            var user = JsonConvert.DeserializeObject<User>(e.Message);
            using (var db = DBHandler.Create())
            {
                var dbUser = db.Users.FirstOrDefault(x => x.Username == user.Username);
                if (dbUser == null)
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    AllSocketInstances[user.Username] = new List<SocketHandler> { socketHandler };
                    SendServerResponse(socketHandler, MessageType.LoginSuccess);
                }
                else SendServerResponse(socketHandler, MessageType.RegistrationFailed);
            }
        }


        private async void SendServerResponse(SocketHandler socketHandler, 
            MessageType messageType, string message = "Server Message")
        {
            try
            {
                await socketHandler.SendMessage(new ChatObject()
                {
                    SenderName = "Server",
                    MessageType = messageType,
                    Message = message
                }) ;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
            }
        }

        

        private async void SendEndToEndMessage(ChatObject e)
        {
            using (var db = DBHandler.Create())
            {
                var receiverInstances = AllSocketInstances[e.ReceiverName];
                if (receiverInstances.Count > 0)
                {
                    foreach (var item in receiverInstances)
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
                db.Add(e);
                await db.SaveChangesAsync();
            }
        }
    }
}

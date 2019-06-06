using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                Console.WriteLine("Logged out instance " + res.Key);
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
            if (!string.IsNullOrWhiteSpace(e.SenderName) && AllSocketInstances[e.SenderName] != null)
            {
                using (var db = DBHandler.Create())
                {
                    var users = await db.Users.ToListAsync();
                    foreach (var user in users)
                    {
                        user.Active = AllSocketInstances.ContainsKey(user.Username);
                    }
                    users = users.OrderByDescending(x => x.Active).ToList();
                    var fullMessage = JsonConvert.SerializeObject(users);
                    SendServerResponse(socketHandler, MessageType.GetUsers, e, fullMessage);
                }
            }
        }

        private async void GetUserHistory(SocketHandler socketHandler, ChatObject original)
        {
            using (var db = DBHandler.Create())
            {
                var skip = 0;
                if (!string.IsNullOrWhiteSpace(original.Message) &&
                            int.TryParse(original.Message, out int res))
                {
                    skip = res;
                }

                var messages = await db.ChatObjects
                                 .Where(x => (x.SenderName == original.SenderName ||
                                            x.SenderName == original.ReceiverName)
                                     && (x.ReceiverName == original.ReceiverName ||
                                        x.ReceiverName == original.SenderName))
                                 .OrderByDescending(m => m.CreatedOn)
                                 .Skip(skip).Take(30)
                                 .ToListAsync();
                var fulMessage = JsonConvert.SerializeObject(messages);
                SendServerResponse(socketHandler, MessageType.GetHistory, original, fulMessage);
            }
        }

        private byte[] GetHash(string inputString)
        {
            using (var algorithm = SHA256.Create())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        private string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private void SubscribeUser(SocketHandler socketHandler, ChatObject e)
        {
            var user = JsonConvert.DeserializeObject<User>(e.Message);
            using (var db = DBHandler.Create())
            {
                var dbUser = db.Users.FirstOrDefault(x => x.Username == user.Username);
                if (dbUser != null && dbUser.StoredPassword == GetHashString(user.Password))
                {
                    if (AllSocketInstances.ContainsKey(dbUser.Username))
                    {
                        AllSocketInstances[dbUser.Username].Add(socketHandler);
                    }
                    else
                    {
                        AllSocketInstances[dbUser.Username] = new List<SocketHandler> { socketHandler };
                    }
                    SendServerResponse(socketHandler, MessageType.LoginSuccess, e);
                }
                else SendServerResponse(socketHandler, MessageType.LoginFailed, e, "Failed to Login User");
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
                    user.StoredPassword = GetHashString(user.Password);
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    AllSocketInstances[user.Username] = new List<SocketHandler> { socketHandler };
                    SendServerResponse(socketHandler, MessageType.LoginSuccess, e);
                }
                else SendServerResponse(socketHandler, MessageType.RegistrationFailed, e, "User exissts");
            }
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

            using (var db = DBHandler.Create())
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

                db.Add(e);
                await db.SaveChangesAsync();
            }
        }
    }
}

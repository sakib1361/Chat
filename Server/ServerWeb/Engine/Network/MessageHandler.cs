using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatCore.Engine;
using Microsoft.AspNetCore.Identity;
using ServerWeb.Engine.Database;

namespace ChatServer.Engine.Network
{
    public class MessageHandler
    {
        private readonly LocalDBContext _localDB;
        private readonly UserManager<IDUser> _usermanager;
        // string being userId with a list of socket instances
        private static readonly Dictionary<string, List<SocketHandler>> AllSocketInstances = new Dictionary<string, List<SocketHandler>>();
        public MessageHandler(LocalDBContext localDB, UserManager<IDUser> userManager)
        {
            _localDB = localDB;
            _usermanager = userManager;
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

        private async void SubscribeUser(SocketHandler socketHandler, ChatObject e)
        {
            var dbUser = await _usermanager.FindByNameAsync(e.SenderName);
            if (dbUser == null)
            {
                SendServerResponse(socketHandler, MessageType.LoginFailed, e, "No User");
            }
            else
            {
                var token = HttpUtility.UrlDecode(e.Message);
                var verify = await _usermanager.VerifyUserTokenAsync(dbUser, "Default", "Chat", token);
                if (verify)
                {
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
                else
                {
                    SendServerResponse(socketHandler, MessageType.LoginFailed, e, "Login unsuccessfull");
                }
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

        internal bool IsLoggedIn(IDUser iDUser)
        {
            return AllSocketInstances.ContainsKey(iDUser.UserName);
        }
    }
}

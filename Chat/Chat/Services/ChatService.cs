﻿using Chat.PlatformService;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chat.Services
{
    public class ChatService
    {
        private readonly ClientHandler ClientHandler;
        readonly ConcurrentDictionary<string, TaskObject> Tasks;
        public ChatService(ClientHandler clientHandler)
        {
            ClientHandler = clientHandler;
            Tasks = new ConcurrentDictionary<string, TaskObject>();
            ClientHandler.MessageRecieveed += ClientHandler_MessageRecieveed;
            ClientHandler.ConnectionChanged += ClientHandler_ClientStateChanged;
        }

        private void ClientHandler_ClientStateChanged(object sender, string e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogService.Instance.Toast(e);
            });
        }

        private void ClientHandler_MessageRecieveed(object sender, ChatObject e)
        {
            if (e.MessageType == MessageType.EndToEnd || e.MessageType == MessageType.BroadCast)
                Device.BeginInvokeOnMainThread(() =>
                {
                    Messenger.Default.Send(e, e.MessageType);
                });
            else
            {
                if (Tasks.TryRemove(e.ChatId, out TaskObject response))
                {
                    if (e.ReceiverName != AppService.CurrentUser)
                    {
                        response.TrySetResult(new ChatObject()
                        {
                            Message = "Invalid Message",
                            SenderName = "Server",
                            ReceiverName = AppService.CurrentUser,
                            MessageType = MessageType.Failed,
                            Id = e.Id,
                            ChatId = e.ChatId
                        });
                    }
                    else
                    {
                        response.TrySetResult(e);
                    }

                }
            }
        }

        internal void Stop()
        {
            ClientHandler?.Close();
        }

        public async Task<bool> SendMessage(ChatObject chatObject)
        {
            try
            {
                await ClientHandler.SendMessage(chatObject);
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return false;
            }
        }

        public async Task<ChatObject> GetData(ChatObject chatObject)
        {
            try
            {
                var packetId = Guid.NewGuid().ToString();
                chatObject.ChatId = packetId;
                var tcs = new TaskObject(packetId, chatObject);
                tcs.MsgFailed += Tcs_MsgFailed;

                Tasks.TryAdd(packetId, tcs);
                if (await SendMessage(chatObject))
                {
                    return await tcs.TaskCompletion.Task;
                }
                else
                {
                    Tasks.TryRemove(packetId, out TaskObject response);
                    tcs.Failed();
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogEngine.Error(ex);
                return null;
            }
        }

        private void Tcs_MsgFailed(object sender, EventArgs e)
        {
            if (sender is TaskObject taskObject)
            {
                if (Tasks.TryRemove(taskObject.Id, out TaskObject response))
                {
                    response.Failed();
                    return;
                }
            }
        }

        public void Start(string address, int port)
        {
            ClientHandler.Connect(address, port);
        }
    }
}

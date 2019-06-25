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
        readonly ConcurrentDictionary<string, TaskObject> Tasks;
        public TestChatService(ClientHandler clientHandler, string username)
        {
            Username = username;
            ClientHandler = clientHandler;
            Tasks = new ConcurrentDictionary<string, TaskObject>();
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
            else
            {
                if (Tasks.TryRemove(e.ChatId, out TaskObject response))
                {
                    if (e.ReceiverName != Username)
                    {
                        response.TrySetResult(new ChatObject()
                        {
                            Message = "Invalid Message",
                            SenderName = "Server",
                            ReceiverName = Username,
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

        public async Task<ChatObject> GetData(ChatObject chatObject)
        {
            try
            {
                var packetId = Guid.NewGuid().ToString();
                chatObject.ChatId = packetId;
                var tcs = new TaskObject(packetId, chatObject);
                tcs.MsgFailed += Tcs_MsgFailed;

                Tasks.TryAdd(packetId, tcs);
                await SendMessage(chatObject);
                return await tcs.TaskCompletion.Task;
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

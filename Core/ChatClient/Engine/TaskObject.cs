using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore.Engine
{
    public class TaskObject
    {
        public string Id { get; }
        public ChatObject ChatObject { get; }
        public TaskCompletionSource<ChatObject> TaskCompletion { get; private set; }
        public bool IsComplete { get; set; }
        public event EventHandler MsgFailed;
        public TaskObject(string id, ChatObject chatObject)
        {
            Id = id;
            ChatObject = chatObject;
            TaskCompletion = new TaskCompletionSource<ChatObject>();
            StartTimer(15000);
        }

        private async void StartTimer(int v)
        {
            await Task.Delay(v);
            if (!IsComplete) MsgFailed?.Invoke(this, null);
        }

        public void TrySetResult(ChatObject e)
        {
            IsComplete = true;
            TaskCompletion.TrySetResult(e);
        }

        public void Failed()
        {
            IsComplete = true;
            TaskCompletion.TrySetResult(new ChatObject
            {
                ChatId = Id,
                Message = "Server Failed",
                SenderName = "Server",
                ReceiverName = ChatObject.ReceiverName,
                MessageType = MessageType.Failed,
            });
        }
    }
}

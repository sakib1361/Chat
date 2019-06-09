using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Engine
{
    [Serializable]
    public class ChatObject
    {
        public int Id { get; set; }
        public string ChatId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public MessageType MessageType { get; set; }
        public bool Delivered { get; set; } = false;

        public ChatObject()
        {

        }
        public ChatObject(MessageType messageType)
        {
            MessageType = messageType;
        }
        public override string ToString()
        {
            return string.Format("{0}=>{1} {2}", SenderName,ReceiverName, Message);
        }

    }

    public enum MessageType
    {
        Register,
        Subscribe,
        GetHistory,
        EndToEnd,
        LoginSuccess,
        LoginFailed,
        RegistrationFailed,
        GetUsers,
        Failed,
        BroadCast
    }
}

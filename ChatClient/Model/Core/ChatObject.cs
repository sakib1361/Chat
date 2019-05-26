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
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public MessageType MessageType { get; set; }
        public bool Delivered { get; set; } = false;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
        GetUsers
    }
}

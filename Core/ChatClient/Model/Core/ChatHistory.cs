using System;
using System.Collections.Generic;
using System.Text;

namespace ChatCore.Engine
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public User FirstUser { get; set; }
        public User SecondUser { get; set; }
        public ICollection<ChatObject> ChatObjects { get; set; }

        public ChatRoom()
        {
            ChatObjects = new HashSet<ChatObject>();
        }
    }
}

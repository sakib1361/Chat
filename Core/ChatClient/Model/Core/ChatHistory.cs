using System;
using System.Collections.Generic;
using System.Text;

namespace ChatCore.Engine
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<ChatObject> ChatObjects { get; set; }

        public ChatRoom()
        {
            ChatObjects = new HashSet<ChatObject>();
            Users = new HashSet<User>();
        }
    }
}

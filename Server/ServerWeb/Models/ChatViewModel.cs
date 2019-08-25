using ChatCore.Engine;
using ChatServer.Engine.Database;
using ChatServer.Engine.Network;
using System.Collections.Generic;
using System.Linq;

namespace ServerWeb.Models
{
    public class ChatViewModel
    {
        public IDUser CurrentUser { get; private set; }
        public List<IDUser> AllUser { get; private set; }
        public List<ChatObject> ChatObjects { get; private set; }

        public ChatViewModel(IDUser user, List<IDUser> users)
        {
            CurrentUser = user;
            AllUser = users.Where(x => x.Id != user.Id).ToList();
        }

        public ChatViewModel(IDUser user, List<ChatObject> chatObjects)
        {
            CurrentUser = user;
            ChatObjects = chatObjects;
        }
    }
}

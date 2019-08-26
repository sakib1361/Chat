using ChatCore.Engine;
using ChatServer.Engine.Database;
using System.Collections.Generic;
using System.Linq;

namespace ServerWeb.Models
{
    public class ChatViewModel
    {
        public ChatObject TemplateChat { get; set; } = new ChatObject();
        public IDUser CurrentUser { get; private set; }
        public IDUser ReceiverUser { get; private set; }
        public List<IDUser> AllUser { get; private set; }
        public string Token { get; private set; }
        public List<ChatObject> ChatObjects { get; private set; }

        public ChatViewModel(IDUser user, List<IDUser> users, string token)
        {
            CurrentUser = user;
            AllUser = users.Where(x => x.Id != user.Id).ToList();
            Token = token;
        }

        public ChatViewModel(IDUser user,IDUser rUser, List<ChatObject> chatObjects)
        {
            CurrentUser = user;
            ReceiverUser = rUser;
            ChatObjects = chatObjects;
        }
    }
}

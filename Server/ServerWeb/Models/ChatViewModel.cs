using ChatCore.Engine;
using System.Collections.Generic;
using System.Linq;

namespace ServerWeb.Models
{
    public class ChatViewModel
    {
        public ChatObject TemplateChat { get; set; } = new ChatObject();
        public User CurrentUser { get; private set; }
        public User ReceiverUser { get; private set; }
        public List<User> AllUser { get; private set; }
        public string Token { get; private set; }
        public List<ChatObject> ChatObjects { get; private set; }

        public ChatViewModel(User user, List<User> users, string token)
        {
            CurrentUser = user;
            AllUser = users.Where(x => x.UserName != user.UserName).ToList();
            Token = token;
        }

        public ChatViewModel(User user, User rUser, List<ChatObject> chatObjects)
        {
            CurrentUser = user;
            ReceiverUser = rUser;
            ChatObjects = chatObjects;
        }
    }
}

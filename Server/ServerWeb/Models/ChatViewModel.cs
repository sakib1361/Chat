using ChatCore.Engine;
using ChatServer.Engine.Database;
using System.Collections.Generic;

namespace ServerWeb.Models
{
    public class ChatViewModel
    {
        public IDUser CurrentUser { get; private set; }
        public List<IDUser> AllUser { get; private set; }

        public ChatViewModel(IDUser user, List<IDUser> users)
        {
            CurrentUser = user;
            AllUser = users;
        }
    }
}

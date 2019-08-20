using ChatCore.Engine;
using System.Collections.Generic;

namespace ServerWeb.Models
{
    public class ChatViewModel
    {
        public User CurrentUser { get; private set; }
        public List<User> AllUser { get; private set; }

        public ChatViewModel(User user, List<User> users)
        {
            CurrentUser = user;
            AllUser = users;
        }
    }
}

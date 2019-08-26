using System.Collections.Generic;
using System.Threading.Tasks;
using ChatClient.Helpers;
using ChatCore.Engine;

namespace ChatClient.Services
{
    public class APIService
    {
        internal Task<string> Register(User user)
        {
            var req = new HttpRequest("register");
            req.AddParameter("username", user.Username);
            req.AddParameter("firstname", user.Firstname);
            req.AddParameter("lastname", user.Lastname);
            req.AddParameter("password", user.Password);
            return HttpWorker.RunWorker<string>(req);
        }

        internal Task<List<User>> GetUsers()
        {
            var req = new HttpRequest("GetUsers");
            return HttpWorker.RunWorker<List<User>>(req);
        }

        internal Task<List<ChatObject>> GetHistory(string receiver)
        {
            var req = new HttpRequest("GetHistory");
            req.AddParameter("username", AppService.CurrentUser);
            req.AddParameter("receivername", receiver);
            return HttpWorker.RunWorker<List<ChatObject>>(req);
        }

        internal Task<string> Login(User user)
        {
            var req = new HttpRequest("login");
            req.AddParameter("username", user.Username);
            req.AddParameter("password", user.Password);
            return HttpWorker.RunWorker<string>(req);
        }
    }
}

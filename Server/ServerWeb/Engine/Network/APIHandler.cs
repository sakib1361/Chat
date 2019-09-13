using ChatCore.Engine;
using ChatCore.Model.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerWeb.Engine.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Engine.Network
{
    public class APIHandler
    {
        private readonly LocalDBContext _localDB;
        private readonly UserManager<IDUser> _manager;
        private readonly SignInManager<IDUser> _signInManager;
        private readonly MessageHandler _msgHandler;

        public APIHandler(
            LocalDBContext localDB,
            MessageHandler messageHandler,
            UserManager<IDUser> manager,
            SignInManager<IDUser> signInManager)
        {
            _localDB = localDB;
            _manager = manager;
            _signInManager = signInManager;
            _msgHandler = messageHandler;
        }

        public async Task<List<ChatObject>> GetUserHistory(string currentUser, string recieverUser, int skip = 0)
        {

            var messages = await _localDB.ChatObjects
                             .Where(x => (x.SenderName == currentUser ||
                                        x.SenderName == recieverUser)
                                 && (x.ReceiverName == recieverUser ||
                                    x.ReceiverName == currentUser))
                             .OrderByDescending(m => m.CreatedOn)
                             .Skip(skip).Take(30)
                             .ToListAsync();
            return messages;
        }

        internal async Task<SignInResult> Login(string userName, string password)
        {
            userName = userName.Trim();
            password = password.Trim();
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            return await _signInManager.PasswordSignInAsync(userName, password, true, true);
        }

        public async Task<List<User>> GetUsers()
        {
            var res = await _manager.GetUsersInRoleAsync(ChatConstants.MemberRole);
            var userList = new List<User>();
            foreach (var item in res)
            {
                userList.Add(new User(item.UserName,item.FirstName,item.LastName)
                {
                    Active = _msgHandler.IsLoggedIn(item),
                });
            }
            return userList;
        }
        internal async Task<IdentityResult> Register(string firstname, string lastname, string username, string password)
        {
            var user = new IDUser
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username
            };
            var res = await _manager.CreateAsync(user, password);
            if (res.Succeeded)
            {
                await _manager.AddToRoleAsync(user, ChatConstants.MemberRole);
            }
            return res;
        }

        internal Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }

        internal async Task<string> GetRole(string user)
        {
            var idUser = await _manager.FindByNameAsync(user);
            if (idUser == null)
            {
                return string.Empty;
            }
            else
            {
                var role = await _manager.GetRolesAsync(idUser);
                return role.FirstOrDefault();
            }
        }
    }
}

using ChatCore.Engine;
using ChatServer.Engine.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Engine.Network
{
    public class APIHandler
    {
        private readonly LocalDBContext _localDB;

        public APIHandler(LocalDBContext localDB)
        {
            _localDB = localDB;
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

        public async Task<List<IDUser>> GetUsers()
        {
            return await _localDB.Users.OfType<IDUser>().ToListAsync();
        }
    }
}

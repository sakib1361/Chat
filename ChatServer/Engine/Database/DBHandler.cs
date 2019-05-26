using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer.Engine.Database
{
    public class DBHandler
    {
        public DBHandler()
        {
            using(var db = new LocalDBContext())
            {
                db.Database.EnsureCreated();
            }
        }

        public LocalDBContext Create()
        {
            return new LocalDBContext();
        }
    }
}

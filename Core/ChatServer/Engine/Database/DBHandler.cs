using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatServer.Engine.Database
{
    public class DBHandler
    {
        private string filePath;

        public DBHandler()
        {
            filePath = Path.Combine(Environment.CurrentDirectory, "chatData.db");
            Console.WriteLine("Server Path " + filePath);
            using (var db = new LocalDBContext(filePath))
            {
                db.Database.EnsureCreated();
            }
        }

        public LocalDBContext Create()
        {
            return new LocalDBContext(filePath);
        }
    }
}

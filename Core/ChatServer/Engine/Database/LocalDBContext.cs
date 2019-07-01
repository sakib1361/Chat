using ChatCore.Engine;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatServer.Engine.Database
{
    public class LocalDBContext : DbContext
    {
        public DbSet<ChatObject> ChatObjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "chatData.db");
            Console.WriteLine("Server Path " + filePath);
            optionsBuilder.UseSqlite("Data Source=" + filePath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Ignore(c => c.Password);
        }
    }
}

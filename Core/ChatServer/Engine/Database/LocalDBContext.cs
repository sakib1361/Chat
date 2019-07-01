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
        private readonly string LocalFile;

        public DbSet<ChatObject> ChatObjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }

        public LocalDBContext(string file)
        {
            LocalFile = file;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + LocalFile);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Ignore(c => c.Password);
        }
    }
}

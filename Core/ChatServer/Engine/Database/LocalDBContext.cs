using ChatCore.Engine;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Engine.Database
{
    public class LocalDBContext : IdentityDbContext
    {
        public DbSet<ChatObject> ChatObjects { get; set; }

        public LocalDBContext()
        {
        }

        public LocalDBContext(DbContextOptions<LocalDBContext> opt) : base(opt)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}

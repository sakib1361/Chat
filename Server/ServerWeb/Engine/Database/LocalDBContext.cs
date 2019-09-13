using ChatCore.Engine;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ServerWeb.Engine.Database
{
    public class LocalDBContext : IdentityDbContext<IDUser>
    {
        public DbSet<ChatObject> ChatObjects { get; set; }

        public LocalDBContext()
        {
        }

        public LocalDBContext(DbContextOptions<LocalDBContext> opt) : base(opt)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Rsa_Application.Database.Entities;

namespace Rsa_Application.Database.Context
{
    public class KeysDbContext : DbContext
    {
        public DbSet<Key> Keys => Set<Key>();
        public KeysDbContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName}" +
                $"\\Rsa Application.Database\\KeysDb.db";
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Key>().HasKey(k => k.Id);
            modelBuilder.Entity<Key>().HasAlternateKey(k => new { k.KeyE, k.KeyN });
        }
    }
}

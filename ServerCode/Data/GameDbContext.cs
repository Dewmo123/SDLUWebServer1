using Microsoft.EntityFrameworkCore;
using ServerCode.Models;

namespace ServerCode.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<PlayerData> PlayerData { get; set; }
        public DbSet<ItemData> ItemData { get; set; }
        public DbSet<PlayerItemData> PlayerItemData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerItemData>()
                .HasKey(pi => new { pi.PlayerId, pi.ItemId });
        }
    }
} 
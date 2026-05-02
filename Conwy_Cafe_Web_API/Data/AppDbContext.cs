using ConwyCafe.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Conwy_Cafe_Web_API.Data
{
    public class AppDbContext : DbContext
    {
        // The constructor passes configuration (like the connection string) to the base DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet represents a table in your SQL Database
        public DbSet<Categories> Categories => Set<Categories>();
        public DbSet<Basket> Baskets => Set<Basket>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<BasketItems> BasketItems => Set<BasketItems>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This is where you define the Composite Key for the BasketItems table
            modelBuilder.Entity<BasketItems>()
                .HasKey(bi => new { bi.BasketId, bi.ItemId });
        }
    }
}

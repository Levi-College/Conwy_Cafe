using ConwyCafe.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Conwy_Cafe_Web_API.Data
{
    public class AppDbContext : DbContext
    {
        // The constructor passes configuration (like the connection string) to the base DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet represents a table in the database, and the type parameter specifies the model that maps to that table.
        //public DbSet<Categories> Categories => Set<Categories>();
        public DbSet<Basket> Baskets => Set<Basket>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<BasketItems> BasketItems => Set<BasketItems>();

        // This is run when the model is created
        // Allows to configure the model (e.g., specify relationships, constraints, conversions) using the ModelBuilder API. This method is called by the Entity Framework when it is setting up the model based on the defined DbSet properties and their corresponding classes.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This tells EF: "Read the VARCHAR from SQL as a String, then turn it into my Enum."
            // This is done as in the C# logic, an enum is used for categories while in the db, they are stored as strings (with constraints)
            modelBuilder.Entity<Basket>()
                .Property(b => b.Category)
                .HasConversion<string>(); 

            // This is where you define the Composite Key for the BasketItems table
            modelBuilder.Entity<BasketItems>()
                .HasKey(bi => new { bi.BasketId, bi.ItemId });
        }

    }
}

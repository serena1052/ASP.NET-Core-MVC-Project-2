using Microsoft.EntityFrameworkCore;
using _1131605.Models;

namespace _1131605.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
        public DbSet<DecisionHistory> DecisionHistories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("TableUsers1131605");
            modelBuilder.Entity<Item>().ToTable("TableItems1131605");
            modelBuilder.Entity<UserAction>().ToTable("TableUserActions1131605");
            modelBuilder.Entity<DecisionHistory>().ToTable("TableDecisionHistories1131605");
            modelBuilder.Entity<Favorite>().ToTable("TableFavorites1131605");
            modelBuilder.Entity<Tag>().ToTable("TableTags1131605");
            modelBuilder.Entity<ItemTag>().ToTable("TableItemTags1131605");

            modelBuilder.Entity<Item>()
                .Property(i => i.ItemId)
                .ValueGeneratedOnAdd(); 

            modelBuilder.Entity<DecisionHistory>()
                .Property(dh => dh.CreatedAt)
                .HasDefaultValueSql("(getdate())");

            modelBuilder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });
        }
    }
}
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
        public DbSet<ProductImage> ProductImages { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
        public DbSet<Variant> Variants => Set<Variant>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
        public DbSet<ColorImage> ColorImages => Set<ColorImage>();
        public DbSet<VariantImage> VariantImages => Set<VariantImage>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug).IsUnique();

            modelBuilder.Entity<ProductGroup>()
                .HasIndex(g => g.ObjectId).IsUnique();

            modelBuilder.Entity<ProductGroup>()
                .HasOne(g => g.Category)
                .WithMany(c => c.ProductGroups)
                .HasForeignKey(g => g.CategoryId);

            modelBuilder.Entity<Variant>()
                .HasIndex(v => v.Sku).IsUnique();

            modelBuilder.Entity<Variant>()
                .HasOne(v => v.ProductGroup)
                .WithMany(g => g.Variants)
                .HasForeignKey(v => v.ProductGroupId);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Variant)
                .WithOne(v => v.Inventory)
                .HasForeignKey<Inventory>(i => i.VariantId);

            modelBuilder.Entity<ColorImage>()
                .HasOne(ci => ci.ProductGroup)
                .WithMany(pg => pg.ColorImages)
                .HasForeignKey(ci => ci.ProductGroupId);

            modelBuilder.Entity<VariantImage>()
                .HasOne(vi => vi.Variant)
                .WithMany(v => v.VariantImages)
                .HasForeignKey(vi => vi.VariantId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNo)
                .IsUnique();

            // Order ↔ OrderItem (1:N)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}

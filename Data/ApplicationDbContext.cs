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
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Inventory> Inventories { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
        public DbSet<ProductImage> ProductImages { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Variant> Variants { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for ProductCategory
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            // Product ↔ ProductCategory
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            // Category ↔ ProductCategory
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // Variant ↔ Inventory (1:1)
            modelBuilder.Entity<Variant>()
                .HasOne(v => v.Inventory)
                .WithOne(i => i.Variant)
                .HasForeignKey<Inventory>(i => i.VariantId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNo)
                .IsUnique();

            // Order ↔ OrderItem (1:N)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);



            // --- Seed Data ---
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    UserName = "admin",
                    PasswordHash = "hashedPassword",
                    PasswordSalt = "salt"
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics",
                    Slug = "electronics",
                    ParentId = null
                },
                new Category
                {
                    Id = 2,
                    Name = "Computers",
                    Slug = "computers",
                    ParentId = 1
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Sku = "LAP123",
                    Brand = "BrandX",
                    Description = "High-end laptop",
                    Specs = "{}",
                    Price = 1499.99m,
                    CreatedUtc = new DateTime(2025, 10, 6, 12, 0, 0),
                    UpdatedUtc = null,
                    IsActive = true,
                    Popularity = 10
                }
            );
        }
    }
}

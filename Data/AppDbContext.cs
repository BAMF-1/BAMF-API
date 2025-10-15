
using Microsoft.EntityFrameworkCore;
using BAMF_API.Models;

namespace BAMF_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Variant> Variants => Set<Variant>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();\n    public DbSet<Order> Orders => Set<Order>();
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
            .HasMany(o => o.Lines)
            .WithOne(l => l.Order)
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

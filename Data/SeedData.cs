
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BAMF_API.Models;

namespace BAMF_API.Data;

public static class SeedData
{
    public static object True { get; private set; }

    public static void EnsureSeeded(ApplicationDbContext db)
    {
        db.Database.Migrate();

        if (db.Categories.Any())
        {
        }
        else
        {
            db.Categories.AddRange(new[] {
                new Category { Name = "Tops", Slug = "tops" },
                new Category { Name = "Pants", Slug = "pants" },
                new Category { Name = "Shoes", Slug = "shoes" },
                new Category { Name = "Accessories", Slug = "accessories" }
            });
            db.SaveChanges();
        }

        if (!db.ProductGroups.Any())
        {
            var pants = db.Categories.First(c => c.Slug == "pants");
            var group = new ProductGroup
            {
                ObjectId = "NIKE-ESSENTIALS-PANTS",
                Name = "Nike Essentials Pants",
                CategoryId = pants.Id,
                Slug = "nike-essentials-pants"
            };
            db.ProductGroups.Add(group);
            db.SaveChanges();

            var s1 = new Variant
            {
                Sku = "NIKE-ESS-PANT-RED-S",
                ProductGroupId = group.Id,
                Color = "Red",
                Size = "S",
                Price = 19.99m
            };
            var s2 = new Variant
            {
                Sku = "NIKE-ESS-PANT-RED-M",
                ProductGroupId = group.Id,
                Color = "Red",
                Size = "M",
                Price = 19.99m
            };
            db.Variants.AddRange(s1, s2);
            db.SaveChanges();

            db.Inventories.AddRange(
                new Inventory { VariantId = s1.Id, Quantity = 2, LowStockThreshold = 1 },
                new Inventory { VariantId = s2.Id, Quantity = 5, LowStockThreshold = 1 }
            );
            db.SaveChanges();

            db.ColorImages.AddRange(
                new ColorImage { ProductGroupId = group.Id, Color = "Red", Url = "https://picsum.photos/seed/red1/600/600", IsPrimary = TrueFalse(True=True) },
                new ColorImage { ProductGroupId = group.Id, Color = "Red", Url = "https://picsum.photos/seed/red2/600/600", SortOrder = 1 }
            );
            db.SaveChanges();
        }
    }

    private static bool TrueFalse(object value)
    {
        throw new NotImplementedException();
    }

    // helper to allow compile with constant
    private static bool TrueFalse(bool True = false) => True;
}

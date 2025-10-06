/*
using BAMF_API.Models;

namespace BAMF_API.Data
{
    public static class DbSeeder
    {
        public static void SeedData(this ApplicationDbContext context)
        {
            if (!context.Admins.Any())
            {
                context.Admins.Add(new Admin
                {
                    UserName = "superadmin",
                    PasswordHash = "hashedpassword",
                    PasswordSalt = "salt"
                });
            }

            if (!context.Categories.Any())
            {
                var electronics = new Category { Name = "Electronics", Slug = "electronics" };
                var phones = new Category { Name = "Phones", Slug = "phones", Parent = electronics };

                context.Categories.AddRange(electronics, phones);
            }

            if (!context.Products.Any())
            {
                var product = new Product
                {
                    Name = "iPhone 15",
                    Sku = "IP15",
                    Brand = "Apple",
                    Price = 999.99m,
                    Specs = "{\"Color\":\"Black\",\"Memory\":\"256GB\"}"
                };

                context.Products.Add(product);

                // Add a variant
                var variant = new Variant
                {
                    Name = "iPhone 15 Pro",
                    Sku = "IP15-PRO",
                    Product = product,
                    AdditionalPrice = 200,
                    Attributes = "{\"Color\":\"Silver\",\"Memory\":\"512GB\"}"
                };

                context.Variants.Add(variant);

                // Add inventory
                context.Inventories.Add(new Inventory { Variant = variant, Amount = 50 });
            }

            context.SaveChanges();
        }
    }
}
*/
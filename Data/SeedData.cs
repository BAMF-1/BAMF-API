using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BAMF_API.Data;

public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext db)
    {
        db.Database.Migrate();

        // Seed Admins
        if (!db.Admins.Any())
        {
            for (int i = 1; i <= 30; i++)
            {
                CreatePasswordHash("Admin123!", out string hash, out string salt);
                db.Admins.Add(new Admin
                {
                    UserName = $"admin{i}",
                    PasswordHash = hash,
                    PasswordSalt = salt
                });
            }
            db.SaveChanges();
        }

        // Seed Users
        if (!db.Users.Any())
        {
            for (int i = 1; i <= 30; i++)
            {
                CreatePasswordHash("User123!", out string hash, out string salt);

                db.Users.Add(new User
                {
                    Email = $"user{i}@example.com",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Cart = "{}"
                });
            }
            db.SaveChanges();
        }


        // Seed Categories
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Tops", Slug = "tops" },
                new Category { Name = "Pants", Slug = "pants" },
                new Category { Name = "Shoes", Slug = "shoes" },
                new Category { Name = "Accessories", Slug = "accessories" }
            );
            db.SaveChanges();
        }

        // Seed ProductGroups and related data
        if (!db.ProductGroups.Any())
        {
            var pants = db.Categories.First(c => c.Slug == "pants");
            var tops = db.Categories.First(c => c.Slug == "tops");
            var shoes = db.Categories.First(c => c.Slug == "shoes");
            var accessories = db.Categories.First(c => c.Slug == "accessories");

            // Product Group 1: Nike Essentials Pants
            var nikeEssentialsPants = new ProductGroup
            {
                ObjectId = "NIKE-ESSENTIALS-PANTS",
                Name = "Nike Essentials Pants",
                CategoryId = pants.Id,
                Slug = "nike-essentials-pants"
            };
            db.ProductGroups.Add(nikeEssentialsPants);
            db.SaveChanges();

            // Variants for Nike Essentials Pants
            var nikePantsRedS = new Variant
            {
                Sku = "NIKE-ESS-PANT-RED-S",
                ProductGroupId = nikeEssentialsPants.Id,
                Color = "Red",
                Size = "S",
                Price = 19.99m
            };
            var nikePantsRedM = new Variant
            {
                Sku = "NIKE-ESS-PANT-RED-M",
                ProductGroupId = nikeEssentialsPants.Id,
                Color = "Red",
                Size = "M",
                Price = 19.99m
            };
            var nikePantsRedL = new Variant
            {
                Sku = "NIKE-ESS-PANT-RED-L",
                ProductGroupId = nikeEssentialsPants.Id,
                Color = "Red",
                Size = "L",
                Price = 19.99m
            };
            var nikePantsBlackS = new Variant
            {
                Sku = "NIKE-ESS-PANT-BLACK-S",
                ProductGroupId = nikeEssentialsPants.Id,
                Color = "Black",
                Size = "S",
                Price = 19.99m
            };
            var nikePantsBlackM = new Variant
            {
                Sku = "NIKE-ESS-PANT-BLACK-M",
                ProductGroupId = nikeEssentialsPants.Id,
                Color = "Black",
                Size = "M",
                Price = 19.99m
            };

            db.Variants.AddRange(nikePantsRedS, nikePantsRedM, nikePantsRedL, nikePantsBlackS, nikePantsBlackM);
            db.SaveChanges();

            // Inventory for Nike Pants
            db.Inventories.AddRange(
                new Inventory { VariantId = nikePantsRedS.Id, Quantity = 10, LowStockThreshold = 2 },
                new Inventory { VariantId = nikePantsRedM.Id, Quantity = 15, LowStockThreshold = 3 },
                new Inventory { VariantId = nikePantsRedL.Id, Quantity = 8, LowStockThreshold = 2 },
                new Inventory { VariantId = nikePantsBlackS.Id, Quantity = 12, LowStockThreshold = 2 },
                new Inventory { VariantId = nikePantsBlackM.Id, Quantity = 20, LowStockThreshold = 3 }
            );
            db.SaveChanges();

            // Color Images for Nike Pants
            db.ColorImages.AddRange(
                new ColorImage { ProductGroupId = nikeEssentialsPants.Id, Color = "Red", Url = "https://picsum.photos/seed/nikered1/600/600", IsPrimary = true, SortOrder = 0 },
                new ColorImage { ProductGroupId = nikeEssentialsPants.Id, Color = "Red", Url = "https://picsum.photos/seed/nikered2/600/600", IsPrimary = false, SortOrder = 1 },
                new ColorImage { ProductGroupId = nikeEssentialsPants.Id, Color = "Black", Url = "https://picsum.photos/seed/nikeblack1/600/600", IsPrimary = true, SortOrder = 0 },
                new ColorImage { ProductGroupId = nikeEssentialsPants.Id, Color = "Black", Url = "https://picsum.photos/seed/nikeblack2/600/600", IsPrimary = false, SortOrder = 1 }
            );
            db.SaveChanges();

            // Product Group 2: Adidas Classic T-Shirt
            var adidasTShirt = new ProductGroup
            {
                ObjectId = "ADIDAS-CLASSIC-TSHIRT",
                Name = "Adidas Classic T-Shirt",
                CategoryId = tops.Id,
                Slug = "adidas-classic-tshirt"
            };
            db.ProductGroups.Add(adidasTShirt);
            db.SaveChanges();

            // Variants for Adidas T-Shirt
            var adidasTShirtWhiteS = new Variant
            {
                Sku = "ADIDAS-TSH-WHITE-S",
                ProductGroupId = adidasTShirt.Id,
                Color = "White",
                Size = "S",
                Price = 24.99m
            };
            var adidasTShirtWhiteM = new Variant
            {
                Sku = "ADIDAS-TSH-WHITE-M",
                ProductGroupId = adidasTShirt.Id,
                Color = "White",
                Size = "M",
                Price = 24.99m
            };
            var adidasTShirtWhiteL = new Variant
            {
                Sku = "ADIDAS-TSH-WHITE-L",
                ProductGroupId = adidasTShirt.Id,
                Color = "White",
                Size = "L",
                Price = 24.99m
            };
            var adidasTShirtBlueM = new Variant
            {
                Sku = "ADIDAS-TSH-BLUE-M",
                ProductGroupId = adidasTShirt.Id,
                Color = "Blue",
                Size = "M",
                Price = 24.99m
            };

            db.Variants.AddRange(adidasTShirtWhiteS, adidasTShirtWhiteM, adidasTShirtWhiteL, adidasTShirtBlueM);
            db.SaveChanges();

            // Inventory for Adidas T-Shirt
            db.Inventories.AddRange(
                new Inventory { VariantId = adidasTShirtWhiteS.Id, Quantity = 25, LowStockThreshold = 5 },
                new Inventory { VariantId = adidasTShirtWhiteM.Id, Quantity = 30, LowStockThreshold = 5 },
                new Inventory { VariantId = adidasTShirtWhiteL.Id, Quantity = 18, LowStockThreshold = 4 },
                new Inventory { VariantId = adidasTShirtBlueM.Id, Quantity = 22, LowStockThreshold = 5 }
            );
            db.SaveChanges();

            // Color Images for Adidas T-Shirt
            db.ColorImages.AddRange(
                new ColorImage { ProductGroupId = adidasTShirt.Id, Color = "White", Url = "https://picsum.photos/seed/adidaswhite1/600/600", IsPrimary = true, AltText = "White Adidas Classic T-Shirt" },
                new ColorImage { ProductGroupId = adidasTShirt.Id, Color = "White", Url = "https://picsum.photos/seed/adidaswhite2/600/600", SortOrder = 1 },
                new ColorImage { ProductGroupId = adidasTShirt.Id, Color = "Blue", Url = "https://picsum.photos/seed/adidasblue1/600/600", IsPrimary = true, AltText = "Blue Adidas Classic T-Shirt" }
            );
            db.SaveChanges();

            // Product Group 3: Running Shoes Pro
            var runningShoes = new ProductGroup
            {
                ObjectId = "RUNNING-SHOES-PRO",
                Name = "Running Shoes Pro",
                CategoryId = shoes.Id,
                Slug = "running-shoes-pro"
            };
            db.ProductGroups.Add(runningShoes);
            db.SaveChanges();

            // Variants for Running Shoes
            var shoesBlack9 = new Variant
            {
                Sku = "RUN-SHOE-BLACK-9",
                ProductGroupId = runningShoes.Id,
                Color = "Black",
                Size = "9",
                Price = 89.99m
            };
            var shoesBlack10 = new Variant
            {
                Sku = "RUN-SHOE-BLACK-10",
                ProductGroupId = runningShoes.Id,
                Color = "Black",
                Size = "10",
                Price = 89.99m
            };
            var shoesGrey10 = new Variant
            {
                Sku = "RUN-SHOE-GREY-10",
                ProductGroupId = runningShoes.Id,
                Color = "Grey",
                Size = "10",
                Price = 89.99m
            };

            db.Variants.AddRange(shoesBlack9, shoesBlack10, shoesGrey10);
            db.SaveChanges();

            // Inventory for Running Shoes
            db.Inventories.AddRange(
                new Inventory { VariantId = shoesBlack9.Id, Quantity = 5, LowStockThreshold = 2 },
                new Inventory { VariantId = shoesBlack10.Id, Quantity = 8, LowStockThreshold = 2 },
                new Inventory { VariantId = shoesGrey10.Id, Quantity = 6, LowStockThreshold = 2 }
            );
            db.SaveChanges();

            // Color Images for Running Shoes
            db.ColorImages.AddRange(
                new ColorImage { ProductGroupId = runningShoes.Id, Color = "Black", Url = "https://picsum.photos/seed/shoesblack1/600/600", IsPrimary = true },
                new ColorImage { ProductGroupId = runningShoes.Id, Color = "Black", Url = "https://picsum.photos/seed/shoesblack2/600/600", SortOrder = 1 },
                new ColorImage { ProductGroupId = runningShoes.Id, Color = "Grey", Url = "https://picsum.photos/seed/shoesgrey1/600/600", IsPrimary = true }
            );
            db.SaveChanges();

            // Product Group 4: Sport Cap
            var sportCap = new ProductGroup
            {
                ObjectId = "SPORT-CAP-CLASSIC",
                Name = "Sport Cap Classic",
                CategoryId = accessories.Id,
                Slug = "sport-cap-classic"
            };
            db.ProductGroups.Add(sportCap);
            db.SaveChanges();

            // Variants for Sport Cap
            var capRedOS = new Variant
            {
                Sku = "CAP-SPORT-RED-OS",
                ProductGroupId = sportCap.Id,
                Color = "Red",
                Size = "One Size",
                Price = 14.99m
            };
            var capBlackOS = new Variant
            {
                Sku = "CAP-SPORT-BLACK-OS",
                ProductGroupId = sportCap.Id,
                Color = "Black",
                Size = "One Size",
                Price = 14.99m
            };

            db.Variants.AddRange(capRedOS, capBlackOS);
            db.SaveChanges();

            // Inventory for Sport Cap
            db.Inventories.AddRange(
                new Inventory { VariantId = capRedOS.Id, Quantity = 50, LowStockThreshold = 10 },
                new Inventory { VariantId = capBlackOS.Id, Quantity = 45, LowStockThreshold = 10 }
            );
            db.SaveChanges();

            // Color Images for Sport Cap
            db.ColorImages.AddRange(
                new ColorImage { ProductGroupId = sportCap.Id, Color = "Red", Url = "https://picsum.photos/seed/capred1/600/600", IsPrimary = true },
                new ColorImage { ProductGroupId = sportCap.Id, Color = "Black", Url = "https://picsum.photos/seed/capblack1/600/600", IsPrimary = true }
            );
            db.SaveChanges();
        }

        /*
        // Seed Legacy Products (if you still need the old Product table)
        if (!db.Products.Any())
        {
            var product1 = new Product
            {
                Name = "Legacy Nike Shoes",
                Sku = "LEGACY-NIKE-001",
                Brand = "Nike",
                Description = "Classic Nike running shoes",
                Price = 79.99m,
                Popularity = 5,
                IsActive = true
            };

            var product2 = new Product
            {
                Name = "Legacy Adidas Jacket",
                Sku = "LEGACY-ADIDAS-002",
                Brand = "Adidas",
                Description = "Comfortable sports jacket",
                Price = 59.99m,
                Popularity = 4,
                IsActive = true
            };

            db.Products.AddRange(product1, product2);
            db.SaveChanges();

            // Product Images
            db.ProductImages.AddRange(
                new ProductImage { ProductId = product1.Id, Url = "https://picsum.photos/seed/legacy1/600/600", AltText = "Legacy Nike Shoes" },
                new ProductImage { ProductId = product2.Id, Url = "https://picsum.photos/seed/legacy2/600/600", AltText = "Legacy Adidas Jacket" }
            );
            db.SaveChanges();
        }
        */

        // Seed Sample Orders
        if (!db.Orders.Any())
        {
            var user = db.Users.FirstOrDefault();

            var order1 = new Order
            {
                OrderNo = "ORD-2025-001",
                UserId = user.Id,
                Email = user.Email,
                Total = 109.97m,
                Status = "Paid",
                CreatedUtc = DateTime.UtcNow.AddDays(-5)
            };

            var order2 = new Order
            {
                OrderNo = "ORD-2025-002",
                Email = "guest@example.com",
                Total = 24.99m,
                Status = "Pending",
                CreatedUtc = DateTime.UtcNow.AddDays(-1)
            };

            db.Orders.AddRange(order1, order2);
            db.SaveChanges();

            // Order Items (using legacy Product for simplicity)
            var legacyProduct = db.Products.FirstOrDefault();
            if (legacyProduct != null)
            {
                db.OrderItems.AddRange(
                    new OrderItem
                    {
                        OrderId = order1.Id,
                        ProductId = legacyProduct.Id,
                        Quantity = 2,
                        UnitPrice = 79.99m
                    }
                );
                db.SaveChanges();
            }
        }

        // Seed Reviews (Updated to use ProductGroups)
        if (!db.Reviews.Any())
        {
            // Get the ProductGroups we created earlier
            var nikeEssentialsPants = db.ProductGroups.First(pg => pg.Slug == "nike-essentials-pants");
            var adidasTShirt = db.ProductGroups.First(pg => pg.Slug == "adidas-classic-tshirt");
            var runningShoes = db.ProductGroups.First(pg => pg.Slug == "running-shoes-pro");
            var sportCap = db.ProductGroups.First(pg => pg.Slug == "sport-cap-classic");

            db.Reviews.AddRange(
                // Reviews for Nike Essentials Pants
                new Review
                {
                    ProductGroupId = nikeEssentialsPants.Id,
                    Rating = 5,
                    Title = "Perfect fit!",
                    Comment = "These pants are amazing! Very comfortable and the material is high quality. I bought them in both red and black."
                },
                new Review
                {
                    ProductGroupId = nikeEssentialsPants.Id,
                    Rating = 4,
                    Title = "Great pants, small sizing",
                    Comment = "Love the quality but they run a bit small. I'd recommend sizing up."
                },
                new Review
                {
                    ProductGroupId = nikeEssentialsPants.Id,
                    Rating = 5,
                    Title = "Excellent for workouts",
                    Comment = "Very comfortable and stylish. Perfect for running and gym sessions."
                },

                // Reviews for Adidas Classic T-Shirt
                new Review
                {
                    ProductGroupId = adidasTShirt.Id,
                    Rating = 5,
                    Title = "Love this shirt!",
                    Comment = "Great quality and fits perfectly! The fabric is soft and breathable."
                },
                new Review
                {
                    ProductGroupId = adidasTShirt.Id,
                    Rating = 4,
                    Title = "Good value",
                    Comment = "Nice shirt for the price. Washes well and holds its shape."
                },

                // Reviews for Running Shoes Pro
                new Review
                {
                    ProductGroupId = runningShoes.Id,
                    Rating = 5,
                    Title = "Best running shoes!",
                    Comment = "These shoes are incredibly comfortable. Great support and cushioning for long runs."
                },
                new Review
                {
                    ProductGroupId = runningShoes.Id,
                    Rating = 4,
                    Title = "Good quality",
                    Comment = "Nice shoes, but a bit pricey. However, the quality justifies the cost."
                },
                new Review
                {
                    ProductGroupId = runningShoes.Id,
                    Rating = 5,
                    Title = "No regrets",
                    Comment = "Worth every penny. My feet don't hurt after long runs anymore."
                },

                // Reviews for Sport Cap
                new Review
                {
                    ProductGroupId = sportCap.Id,
                    Rating = 5,
                    Title = "Perfect cap",
                    Comment = "Great fit and looks stylish. The one size fits all actually works!"
                },
                new Review
                {
                    ProductGroupId = sportCap.Id,
                    Rating = 4,
                    Title = "Nice accessory",
                    Comment = "Good quality cap. The adjustable strap makes it comfortable for everyone."
                }
            );
            db.SaveChanges();
        }
    }

    private static void CreatePasswordHash(string password, out string hash, out string salt)
    {
        using var hmac = new HMACSHA512();
        salt = Convert.ToBase64String(hmac.Key);
        hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
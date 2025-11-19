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

            // ========== TOPS ==========

            // 1. Nike Dri-FIT Training Top
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "NIKE-DRIFIT-TRAINING",
                Name = "Nike Dri-FIT Training Top",
                CategoryId = tops.Id,
                Slug = "nike-drifit-training-top",
                Variants = new[]
                {
                    new VariantData("NIKE-TRAIN-BLACK-S", "Black", "S", 34.99m, 20, 5),
                    new VariantData("NIKE-TRAIN-BLACK-M", "Black", "M", 34.99m, 30, 5),
                    new VariantData("NIKE-TRAIN-BLACK-L", "Black", "L", 34.99m, 25, 5),
                    new VariantData("NIKE-TRAIN-NAVY-M", "Navy", "M", 34.99m, 18, 5),
                    new VariantData("NIKE-TRAIN-NAVY-L", "Navy", "L", 34.99m, 15, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://cdn.sportshop.com/catalog/product/1500/1500/1/0/103761_1.jpg", true, 0),
                    new ColorImageData("Black", "https://www.asos.com/se/nike-training/nike-training-legend-dri-fit-svart-t-shirt/prd/203655255", false, 1),
                    new ColorImageData("Navy", "https://thumblr.uniid.it/product/384513/eb785871fa75.jpg?width=3840&format=webp&q=75", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Perfect for workouts", "Keeps me dry during intense training sessions. The fit is perfect!"),
                    new ReviewData(4, "Good quality", "Nice material but runs slightly small. Order one size up."),
                    new ReviewData(5, "Love it!", "Best training top I've owned. Very comfortable and breathable."),
                }
            });

            // 2. Adidas Performance Tee
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "ADIDAS-PERFORMANCE-TEE",
                Name = "Adidas Performance Tee",
                CategoryId = tops.Id,
                Slug = "adidas-performance-tee",
                Variants = new[]
                {
                    new VariantData("ADIDAS-PERF-WHITE-S", "White", "S", 29.99m, 25, 5),
                    new VariantData("ADIDAS-PERF-WHITE-M", "White", "M", 29.99m, 35, 7),
                    new VariantData("ADIDAS-PERF-WHITE-L", "White", "L", 29.99m, 28, 5),
                    new VariantData("ADIDAS-PERF-GREY-M", "Grey", "M", 29.99m, 22, 5),
                    new VariantData("ADIDAS-PERF-GREY-L", "Grey", "L", 29.99m, 20, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("White", "https://img01.ztat.net/article/spp-media-p1/0d12e12795aa4aa28bf2c994b607b7f3/8d399e3f6a794326b2e0f9b446cc8602.jpg?imwidth=1800", true, 0),
                    new ColorImageData("White", "https://image-resizing.booztcdn.com/adidas/adiin5164_cwhite.webp?has_grey=1&has_webp=1&version=5bd1410804653deb90626d6b1775f2ec&dpr=2.5&size=w400", false, 1),
                    new ColorImageData("Grey", "https://cdn.kids-world.com/images/products/FY369/681px/FY369.jpg?v=1702988077", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Great everyday tee", "Comfortable and versatile. Wear it everywhere!"),
                    new ReviewData(5, "Excellent quality", "Washes well and maintains its shape after many washes."),
                    new ReviewData(4, "Nice shirt", "Good value for money. Material could be slightly thicker."),
                }
            });

            // 3. Puma Essential Logo Tee
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "PUMA-ESSENTIAL-TEE",
                Name = "Puma Essential Logo Tee",
                CategoryId = tops.Id,
                Slug = "puma-essential-logo-tee",
                Variants = new[]
                {
                    new VariantData("PUMA-ESS-BLACK-S", "Black", "S", 24.99m, 30, 6),
                    new VariantData("PUMA-ESS-BLACK-M", "Black", "M", 24.99m, 40, 8),
                    new VariantData("PUMA-ESS-BLACK-L", "Black", "L", 24.99m, 32, 6),
                    new VariantData("PUMA-ESS-RED-M", "Red", "M", 24.99m, 25, 5),
                    new VariantData("PUMA-ESS-RED-L", "Red", "L", 24.99m, 20, 5),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://photos6.spartoo.se/photos/198/19814213/19814213_1200_A.jpg", true, 0),
                    new ColorImageData("Red", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTML4WoiUEKplpdIDbkf_BAV6ZJDRtcmrm0dQ&s", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Love Puma!", "Perfect fit and the logo looks great. Highly recommend!"),
                    new ReviewData(4, "Good basic tee", "Nothing fancy but does the job well. Comfortable to wear."),
                }
            });

            // 4. Under Armour Tech 2.0
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "UA-TECH-20",
                Name = "Under Armour Tech 2.0 Tee",
                CategoryId = tops.Id,
                Slug = "under-armour-tech-20",
                Variants = new[]
                {
                    new VariantData("UA-TECH-BLUE-S", "Blue", "S", 27.99m, 18, 4),
                    new VariantData("UA-TECH-BLUE-M", "Blue", "M", 27.99m, 28, 5),
                    new VariantData("UA-TECH-BLUE-L", "Blue", "L", 27.99m, 22, 5),
                    new VariantData("UA-TECH-GREEN-M", "Green", "M", 27.99m, 15, 3),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Blue", "https://03.cdn37.se/3OL/images/2.449651/under-armour-t-shirt-tech-20-ss-youth-ether-blue.jpeg", true, 0),
                    new ColorImageData("Green", "https://i1.t4s.cz//products/1363284-300/under-armour-tech-2-0-736781-1363284-300-960.webp", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "UA never disappoints", "Super comfortable and great moisture wicking. Perfect for gym."),
                    new ReviewData(5, "Best athletic tee", "Fits true to size and feels amazing during workouts."),
                    new ReviewData(4, "Solid choice", "Good quality but a bit pricey compared to alternatives."),
                }
            });

            // 5. Reebok Classic Cotton Tee
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "REEBOK-CLASSIC-TEE",
                Name = "Reebok Classic Cotton Tee",
                CategoryId = tops.Id,
                Slug = "reebok-classic-cotton-tee",
                Variants = new[]
                {
                    new VariantData("REEBOK-CLAS-WHITE-S", "White", "S", 22.99m, 35, 7),
                    new VariantData("REEBOK-CLAS-WHITE-M", "White", "M", 22.99m, 45, 9),
                    new VariantData("REEBOK-CLAS-WHITE-L", "White", "L", 22.99m, 38, 7),
                    new VariantData("REEBOK-CLAS-BLACK-M", "Black", "M", 22.99m, 40, 8),
                },
                ColorImages = new[]
                {
                    new ColorImageData("White", "https://www.reebok.com/cdn/shop/files/100054977_F_Model_eCom-tif.png?v=1734091946&width=2000", true, 0),
                    new ColorImageData("Black", "https://img2.ans-media.com/i/840x1260/AA00-TSM0LK-99X_F1.jpg?v=1683630529", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(4, "Comfortable cotton", "Soft and breathable. Great for everyday wear."),
                    new ReviewData(5, "Classic design", "Simple and timeless. Goes with everything!"),
                }
            });

            // ========== PANTS ==========

            // 6. Nike Essentials Joggers
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "NIKE-ESSENTIALS-JOGGERS",
                Name = "Nike Essentials Joggers",
                CategoryId = pants.Id,
                Slug = "nike-essentials-joggers",
                Variants = new[]
                {
                    new VariantData("NIKE-JOG-BLACK-S", "Black", "S", 54.99m, 15, 3),
                    new VariantData("NIKE-JOG-BLACK-M", "Black", "M", 54.99m, 22, 5),
                    new VariantData("NIKE-JOG-BLACK-L", "Black", "L", 54.99m, 18, 4),
                    new VariantData("NIKE-JOG-GREY-M", "Grey", "M", 54.99m, 20, 4),
                    new VariantData("NIKE-JOG-GREY-L", "Grey", "L", 54.99m, 16, 3),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://img01.ztat.net/article/spp-media-p1/514fcaeb9d774de0aa942ec5e7870bbc/5e737ac0ec384563b10df48b2b7eef0e.jpg?imwidth=762", true, 0),
                    new ColorImageData("Black", "https://images.asos-media.com/products/nike-essentials-svarta-smala-joggers/11690783-1-blackwhite?$n_750w$&wid=750&hei=750&fit=crop", false, 1),
                    new ColorImageData("Grey", "https://xcdn.next.co.uk/common/items/default/default/itemimages/3_4Ratio/product/lge/K88982s.jpg?im=Resize,width=750", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Super comfy!", "These joggers are amazing for lounging and light exercise."),
                    new ReviewData(5, "Best purchase", "Perfect fit, great material. Worth every penny!"),
                    new ReviewData(4, "Nice joggers", "Comfortable but wish they had pockets on both sides."),
                    new ReviewData(5, "Love them!", "Bought two pairs because they're so good. Highly recommended!"),
                }
            });

            // 7. Adidas Tiro Track Pants
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "ADIDAS-TIRO-TRACK",
                Name = "Adidas Tiro Track Pants",
                CategoryId = pants.Id,
                Slug = "adidas-tiro-track-pants",
                Variants = new[]
                {
                    new VariantData("ADIDAS-TIRO-BLACK-S", "Black", "S", 49.99m, 18, 4),
                    new VariantData("ADIDAS-TIRO-BLACK-M", "Black", "M", 49.99m, 25, 5),
                    new VariantData("ADIDAS-TIRO-BLACK-L", "Black", "L", 49.99m, 20, 4),
                    new VariantData("ADIDAS-TIRO-NAVY-M", "Navy", "M", 49.99m, 15, 3),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://assets.adidas.com/images/w_600,f_auto,q_auto/a476aac9a086414c830fac28011193ff_9366/Tiro_21_Track_Pants_Svart_GM7374_01_laydown.jpg", true, 0),
                    new ColorImageData("Navy", "https://assets.adidas.com/images/w_600,f_auto,q_auto/0317af1dea8647ffb858ac310103c462_9366/Tiro_21_Track_Pants_Bla_GE5425_01_laydown.jpg", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Classic Adidas quality", "The three stripes never disappoint. Perfect for training."),
                    new ReviewData(4, "Good track pants", "Comfortable and stylish. Sizing is accurate."),
                    new ReviewData(5, "Love the fit", "Tapered leg design looks great. Very satisfied!"),
                }
            });

            // 8. Puma Tapered Training Pants
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "PUMA-TAPERED-TRAINING",
                Name = "Puma Tapered Training Pants",
                CategoryId = pants.Id,
                Slug = "puma-tapered-training-pants",
                Variants = new[]
                {
                    new VariantData("PUMA-TRAIN-BLACK-S", "Black", "S", 44.99m, 20, 4),
                    new VariantData("PUMA-TRAIN-BLACK-M", "Black", "M", 44.99m, 30, 6),
                    new VariantData("PUMA-TRAIN-BLACK-L", "Black", "L", 44.99m, 25, 5),
                    new VariantData("PUMA-TRAIN-GREY-M", "Grey", "M", 44.99m, 18, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://img01.ztat.net/article/spp-media-p1/27e480b21c074048a51057883c0f20ed/d211ab91c9e2473a95d1a5ffdcb31f4f.jpg?imwidth=1800", true, 0),
                    new ColorImageData("Grey", "https://img01.ztat.net/article/spp-media-p1/a59438a77fa546b2868d51bce08dc37d/b03e96dc9eac4e23b8fb8b511e68b3ea.jpg?imwidth=1800", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Great value", "Quality pants at a reasonable price. Very happy!"),
                    new ReviewData(4, "Comfortable fit", "Nice material and good build quality. Recommend!"),
                }
            });

            // 9. Under Armour Sportstyle Joggers
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "UA-SPORTSTYLE-JOGGERS",
                Name = "Under Armour Sportstyle Joggers",
                CategoryId = pants.Id,
                Slug = "ua-sportstyle-joggers",
                Variants = new[]
                {
                    new VariantData("UA-SPORT-GREY-S", "Grey", "S", 59.99m, 12, 3),
                    new VariantData("UA-SPORT-GREY-M", "Grey", "M", 59.99m, 18, 4),
                    new VariantData("UA-SPORT-GREY-L", "Grey", "L", 59.99m, 15, 3),
                    new VariantData("UA-SPORT-BLACK-M", "Black", "M", 59.99m, 20, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Grey", "https://xcdn.next.co.uk/common/items/default/default/itemimages/3_4Ratio/product/lge/106470s2.jpg?im=Resize,width=750", true, 0),
                    new ColorImageData("Black", "https://image-resizing.booztcdn.com/under-armour/uar1290261_cblack_v001.webp?has_grey=1&has_webp=1&version=c4872751af43f8f30778b7218dc85234&dpr=2.5&size=w400", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Premium quality", "You can feel the quality. These are built to last!"),
                    new ReviewData(5, "Perfect joggers", "Comfortable, stylish, and functional. What more could you want?"),
                    new ReviewData(4, "Good but pricey", "Great quality but a bit expensive compared to others."),
                }
            });

            // 10. Reebok Workout Ready Pants
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "REEBOK-WORKOUT-PANTS",
                Name = "Reebok Workout Ready Pants",
                CategoryId = pants.Id,
                Slug = "reebok-workout-ready-pants",
                Variants = new[]
                {
                    new VariantData("REEBOK-WORK-BLACK-S", "Black", "S", 39.99m, 22, 5),
                    new VariantData("REEBOK-WORK-BLACK-M", "Black", "M", 39.99m, 30, 6),
                    new VariantData("REEBOK-WORK-BLACK-L", "Black", "L", 39.99m, 25, 5),
                    new VariantData("REEBOK-WORK-NAVY-M", "Navy", "M", 39.99m, 18, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://i5.walmartimages.com/seo/Reebok-Men-s-Workout-Ready-Track-Pant_131a6e2f-4781-4f87-b9df-c4fb1360efe6.1717f7ab7d991cb4b6ba4fe1c8d0dbaa.jpeg", true, 0),
                    new ColorImageData("Navy", "https://www.reebok.eu/cdn/shop/files/19728605_44843699_800.webp?v=1744631065&width=600", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(4, "Good workout pants", "Comfortable and flexible. Great for gym sessions."),
                    new ReviewData(5, "Excellent!", "These pants are perfect for any type of exercise. Love them!"),
                }
            });

            // ========== SHOES ==========

            // 11. Nike Air Max Impact
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "NIKE-AIR-MAX-IMPACT",
                Name = "Nike Air Max Impact",
                CategoryId = shoes.Id,
                Slug = "nike-air-max-impact",
                Variants = new[]
                {
                    new VariantData("NIKE-AIR-BLACK-9", "Black", "9", 124.99m, 8, 2),
                    new VariantData("NIKE-AIR-BLACK-10", "Black", "10", 124.99m, 10, 2),
                    new VariantData("NIKE-AIR-BLACK-11", "Black", "11", 124.99m, 7, 2),
                    new VariantData("NIKE-AIR-WHITE-10", "White", "10", 124.99m, 6, 2),
                    new VariantData("NIKE-AIR-WHITE-11", "White", "11", 124.99m, 5, 2),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://m.media-amazon.com/images/I/31bqi95bKuL._SY900_.jpg", true, 0),
                    new ColorImageData("Black", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQNJ7ZkTJPMxdi132ua0AFNv78Ks1BxPwG_uQ&s", false, 1),
                    new ColorImageData("White", "https://m.media-amazon.com/images/I/31VUiIwVgyL._AC_SY300_.jpg", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Incredible cushioning!", "Best running shoes I've ever owned. The Air Max technology is amazing!"),
                    new ReviewData(5, "Worth every cent", "Expensive but absolutely worth it. My feet have never been happier."),
                    new ReviewData(4, "Great shoes", "Very comfortable but took a few days to break in."),
                    new ReviewData(5, "Love them!", "Perfect for running and everyday wear. Highly recommended!"),
                }
            });

            // 12. Adidas Ultraboost 22
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "ADIDAS-ULTRABOOST-22",
                Name = "Adidas Ultraboost 22",
                CategoryId = shoes.Id,
                Slug = "adidas-ultraboost-22",
                Variants = new[]
                {
                    new VariantData("ADIDAS-ULTRA-BLACK-9", "Black", "9", 179.99m, 5, 1),
                    new VariantData("ADIDAS-ULTRA-BLACK-10", "Black", "10", 179.99m, 6, 2),
                    new VariantData("ADIDAS-ULTRA-WHITE-10", "White", "10", 179.99m, 4, 1),
                    new VariantData("ADIDAS-ULTRA-WHITE-11", "White", "11", 179.99m, 3, 1),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://www.loparshop.se/media/catalog/product/cache/e1bfa30f5f000aa573b2ee969a7a0fde/1/5/1500x1500_-_2022-03-04t095223.111.jpg", true, 0),
                    new ColorImageData("White", "https://cdn-images.farfetch-contents.com/19/95/75/28/19957528_44761448_600.jpg", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Premium running shoe", "The boost technology is incredible. Worth the investment!"),
                    new ReviewData(5, "Best I've tried", "Amazing comfort and support for long distance running."),
                    new ReviewData(4, "Excellent quality", "Great shoes but very expensive. Save up for these!"),
                }
            });

            // 13. Puma RS-X Sneakers
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "PUMA-RSX-SNEAKERS",
                Name = "Puma RS-X Sneakers",
                CategoryId = shoes.Id,
                Slug = "puma-rsx-sneakers",
                Variants = new[]
                {
                    new VariantData("PUMA-RSX-WHITE-9", "White", "9", 89.99m, 12, 3),
                    new VariantData("PUMA-RSX-WHITE-10", "White", "10", 89.99m, 15, 3),
                    new VariantData("PUMA-RSX-WHITE-11", "White", "11", 89.99m, 10, 2),
                    new VariantData("PUMA-RSX-BLACK-10", "Black", "10", 89.99m, 14, 3),
                },
                ColorImages = new[]
                {
                    new ColorImageData("White", "https://img01.ztat.net/article/spp-media-p1/5df6be23d680434eaf23f46cb6cd0632/694e48117b9c41bda541d71f52909d5b.jpg?imwidth=1800", true, 0),
                    new ColorImageData("Black", "https://images.puma.com/image/upload/f_auto,q_auto,b_rgb:fafafa,w_2000,h_2000/global/389896/01/sv01/fnd/PNA/fmt/png/RS-X-PEB-Sneakers", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Stylish and comfortable", "Love the retro design. Great for casual wear!"),
                    new ReviewData(4, "Good sneakers", "Comfortable and look great. True to size."),
                    new ReviewData(5, "Perfect!", "These sneakers are amazing. Get compliments all the time!"),
                }
            });

            // 14. New Balance 990v5
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "NB-990V5",
                Name = "New Balance 990v5",
                CategoryId = shoes.Id,
                Slug = "new-balance-990v5",
                Variants = new[]
                {
                    new VariantData("NB-990-GREY-9", "Grey", "9", 174.99m, 6, 2),
                    new VariantData("NB-990-GREY-10", "Grey", "10", 174.99m, 8, 2),
                    new VariantData("NB-990-GREY-11", "Grey", "11", 174.99m, 5, 1),
                    new VariantData("NB-990-NAVY-10", "Navy", "10", 174.99m, 7, 2),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Grey", "https://nb.scene7.com/is/image/NB/w990gl5_nb_02_i?$dw_detail_gallery$", true, 0),
                    new ColorImageData("Navy", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR3D7nBQ092iOZ2mDaxUEvZkPYS42SQdvWHHw&s", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Classic NB quality", "Made in USA quality. These shoes are built to last!"),
                    new ReviewData(5, "Worth the price", "Incredibly comfortable. Best shoes for all-day wear."),
                    new ReviewData(4, "Great shoes", "Premium quality but quite expensive. You get what you pay for."),
                }
            });

            // 15. Asics Gel-Kayano 29
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "ASICS-KAYANO-29",
                Name = "Asics Gel-Kayano 29",
                CategoryId = shoes.Id,
                Slug = "asics-gel-kayano-29",
                Variants = new[]
                {
                    new VariantData("ASICS-KAY-BLACK-9", "Black", "9", 159.99m, 7, 2),
                    new VariantData("ASICS-KAY-BLACK-10", "Black", "10", 159.99m, 9, 2),
                    new VariantData("ASICS-KAY-BLUE-10", "Blue", "10", 159.99m, 6, 2),
                    new VariantData("ASICS-KAY-BLUE-11", "Blue", "11", 159.99m, 5, 1),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://images.asics.com/is/image/asics/1012B272_002_SB_FR_GLB?$sfcc-product$", true, 0),
                    new ColorImageData("Blue", "https://www.loparshop.se/media/catalog/product/cache/e1bfa30f5f000aa573b2ee969a7a0fde/1/0/1011b471_402_sr_rt_glb_png_1500x1500-jpg.jpg", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Best for stability", "Perfect for overpronators. Excellent support and cushioning!"),
                    new ReviewData(5, "Running game changer", "These shoes transformed my running experience. No more pain!"),
                    new ReviewData(4, "Solid choice", "Great running shoes. A bit heavy but very supportive."),
                }
            });

            // ========== ACCESSORIES ==========

            // 16. Nike Heritage Crossbody Bag
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "NIKE-HERITAGE-BAG",
                Name = "Nike Heritage Crossbody Bag",
                CategoryId = accessories.Id,
                Slug = "nike-heritage-crossbody-bag",
                Variants = new[]
                {
                    new VariantData("NIKE-BAG-BLACK-OS", "Black", "One Size", 24.99m, 40, 8),
                    new VariantData("NIKE-BAG-GREY-OS", "Grey", "One Size", 24.99m, 35, 7),
                    new VariantData("NIKE-BAG-NAVY-OS", "Navy", "One Size", 24.99m, 30, 6),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://xcdn.next.co.uk/common/items/default/default/itemimages/3_4Ratio/product/lge/829247s.jpg?im=Resize,width=750", true, 0),
                    new ColorImageData("Grey", "https://xcdn.next.co.uk/common/items/default/default/itemimages/3_4Ratio/product/lge/K84175s.jpg?im=Resize,width=750", true, 0),
                    new ColorImageData("Navy", "https://thumblr.uniid.it/product/368791/f789c8d6a914.jpg?width=3840&format=webp&q=75", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Perfect small bag", "Great for carrying essentials. Love the compact size!"),
                    new ReviewData(4, "Good quality", "Nice bag for the price. Holds phone, wallet, and keys perfectly."),
                    new ReviewData(5, "Love it!", "Stylish and practical. Use it every day!"),
                }
            });

            // 17. Adidas Baseball Cap
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "ADIDAS-BASEBALL-CAP",
                Name = "Adidas Baseball Cap",
                CategoryId = accessories.Id,
                Slug = "adidas-baseball-cap",
                Variants = new[]
                {
                    new VariantData("ADIDAS-CAP-BLACK-OS", "Black", "One Size", 19.99m, 50, 10),
                    new VariantData("ADIDAS-CAP-WHITE-OS", "White", "One Size", 19.99m, 45, 9),
                    new VariantData("ADIDAS-CAP-NAVY-OS", "Navy", "One Size", 19.99m, 40, 8),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://assets.adidas.com/images/w_600,f_auto,q_auto/34e52694ddcc4c908428c2f875fff3ab_9366/Keps_Svart_JC6047_01_00_standard.jpg", true, 0),
                    new ColorImageData("White", "https://m.media-amazon.com/images/I/21JIk1X5dbL._AC_.jpg", true, 0),
                    new ColorImageData("Navy", "https://img01.ztat.net/article/spp-media-p1/2efed8de2584456a8dcbcb7f5f85310d/b72dcaba244146f0b5784b1b99df4f33.jpg?imwidth=1800&filter=packshot", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Classic cap", "Simple design, great quality. Perfect everyday cap!"),
                    new ReviewData(5, "Love it!", "Fits perfectly and looks great. Three stripes FTW!"),
                    new ReviewData(4, "Good cap", "Nice quality for the price. Adjustable strap works well."),
                }
            });

            // 18. Puma Gym Duffle Bag
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "PUMA-GYM-DUFFLE",
                Name = "Puma Gym Duffle Bag",
                CategoryId = accessories.Id,
                Slug = "puma-gym-duffle-bag",
                Variants = new[]
                {
                    new VariantData("PUMA-DUFFLE-BLACK-OS", "Black", "One Size", 34.99m, 25, 5),
                    new VariantData("PUMA-DUFFLE-RED-OS", "Red", "One Size", 34.99m, 20, 4),
                    new VariantData("PUMA-DUFFLE-GREY-OS", "Grey", "One Size", 34.99m, 22, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://www.tradeinn.com/f/13720/137202227/puma-gym-duffle-m-bag.webp", true, 0),
                    new ColorImageData("Red", "https://img01.ztat.net/article/spp-media-p1/4fe7f918ada5465e85fa5fc85480b861/bf1e03da255c44c6a72d8909f821adcb.jpg?imwidth=1800&filter=packshot", true, 0),
                    new ColorImageData("Grey", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ_2tOr_OfjJE6D-wsSOGiJvklofnp7rS5qPA&s", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Perfect gym bag", "Spacious and durable. Fits all my gym gear easily!"),
                    new ReviewData(4, "Good bag", "Nice quality and size. Shoulder strap could be more padded."),
                    new ReviewData(5, "Great purchase", "Love this bag! Very practical and looks good too."),
                }
            });

            // 19. Under Armour Sports Socks (3-Pack)
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "UA-SPORTS-SOCKS-3PK",
                Name = "Under Armour Sports Socks (3-Pack)",
                CategoryId = accessories.Id,
                Slug = "ua-sports-socks-3pack",
                Variants = new[]
                {
                    new VariantData("UA-SOCK-WHITE-M", "White", "M", 16.99m, 60, 12),
                    new VariantData("UA-SOCK-WHITE-L", "White", "L", 16.99m, 55, 11),
                    new VariantData("UA-SOCK-BLACK-M", "Black", "M", 16.99m, 65, 13),
                    new VariantData("UA-SOCK-BLACK-L", "Black", "L", 16.99m, 58, 12),
                },
                ColorImages = new[]
                {
                    new ColorImageData("White", "https://cdn.mainlinemenswear.co.uk/w_900,h_900/f_auto,q_auto/mainlinemenswear/shopimages/products/185924/Mainimage.jpg", true, 0),
                    new ColorImageData("Black", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQGlnhvgnzStMYS2BHhG5txzxkFlMYF_HDz3Q&s", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Best socks!", "Comfortable and durable. Don't slip during workouts."),
                    new ReviewData(5, "Great quality", "These socks are amazing. Bought multiple packs!"),
                    new ReviewData(4, "Good value", "Quality socks at a decent price. Happy with purchase."),
                }
            });

            // 20. Nike Headband
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "NIKE-HEADBAND",
                Name = "Nike Swoosh Headband",
                CategoryId = accessories.Id,
                Slug = "nike-swoosh-headband",
                Variants = new[]
                {
                    new VariantData("NIKE-HEAD-BLACK-OS", "Black", "One Size", 8.99m, 80, 16),
                    new VariantData("NIKE-HEAD-WHITE-OS", "White", "One Size", 8.99m, 75, 15),
                    new VariantData("NIKE-HEAD-RED-OS", "Red", "One Size", 8.99m, 70, 14),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://cdn.courtside.se/_files/productmedia/3051_35207.jpg", true, 0),
                    new ColorImageData("White", "https://cdn.courtside.se/_files/productmedia/4516_35206.jpg", true, 0),
                    new ColorImageData("Red", "https://www.mistertennis.com/images/2021-media/nike-swoosh-headband-red-white-n-nn-07-601-os_A.jpg", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Perfect for workouts", "Keeps hair and sweat out of my face. Essential gym gear!"),
                    new ReviewData(4, "Good headband", "Does the job well. Stays in place during exercise."),
                    new ReviewData(5, "Love it!", "Simple and effective. Bought one in each color!"),
                }
            });

            // 21. Reebok Training Backpack
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "REEBOK-TRAINING-BACKPACK",
                Name = "Reebok Training Backpack",
                CategoryId = accessories.Id,
                Slug = "reebok-training-backpack",
                Variants = new[]
                {
                    new VariantData("REEBOK-BP-BLACK-OS", "Black", "One Size", 44.99m, 20, 4),
                    new VariantData("REEBOK-BP-GREY-OS", "Grey", "One Size", 44.99m, 18, 4),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://img01.ztat.net/article/spp-media-p1/698fcaf5585d416381f19ad5557775b1/d744790058ac427ea8ab8c013be1d5a4.jpg?imwidth=1800&filter=packshot", true, 0),
                    new ColorImageData("Grey", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQmHPmLGy67PTLREXJ6X8WQuMpyv3Fa6Cw1Ww&s", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Excellent backpack", "Lots of compartments and very durable. Perfect for gym and work!"),
                    new ReviewData(4, "Good quality", "Spacious and comfortable to carry. Recommend!"),
                }
            });

            // 22. Adidas Water Bottle (750ml)
            CreateProductWithVariants(db, new ProductGroupData
            {
                ObjectId = "ADIDAS-WATER-BOTTLE",
                Name = "Adidas Performance Bottle 750ml",
                CategoryId = accessories.Id,
                Slug = "adidas-performance-bottle",
                Variants = new[]
                {
                    new VariantData("ADIDAS-BOTTLE-BLACK-OS", "Black", "One Size", 12.99m, 100, 20),
                    new VariantData("ADIDAS-BOTTLE-BLUE-OS", "Blue", "One Size", 12.99m, 95, 19),
                    new VariantData("ADIDAS-BOTTLE-WHITE-OS", "White", "One Size", 12.99m, 90, 18),
                },
                ColorImages = new[]
                {
                    new ColorImageData("Black", "https://assets.adidas.com/images/w_600,f_auto,q_auto/dffb7992446842a98f73aa860118cfee_9366/Performance_Bottle_750_ML_Svart_FM9931_01_00_standard.jpg", true, 0),
                    new ColorImageData("Blue", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTqWt8Ef0pOzGhEBB5ybmpsQMSRA2W6IrMnLA&s", true, 0),
                    new ColorImageData("White", "https://assets.adidas.com/images/w_600,f_auto,q_auto/c5f49bde493c45b0ad91aa8601192790_9366/Performance_Bottle_750_ML_Vit_FM9932_00_plp_standard.jpg", true, 0),
                },
                Reviews = new[]
                {
                    new ReviewData(5, "Great bottle", "Leak-proof and easy to clean. Perfect for gym!"),
                    new ReviewData(4, "Good value", "Nice design and does the job. Good purchase."),
                    new ReviewData(5, "Love it!", "Keeps water cold and doesn't leak. Highly recommend!"),
                }
            });
        }

        // Seed Sample Orders
        if (!db.Orders.Any())
        {
            var users = db.Users.Take(5).ToList();
            var variants = db.Variants.Include(v => v.ProductGroup).ToList();
            var random = new Random(42); // Fixed seed for reproducibility

            for (int i = 1; i <= 20; i++)
            {
                var user = users[random.Next(users.Count)];
                var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 60));
                var statuses = new[] { "Paid", "Paid", "Paid", "Shipped", "Delivered", "Pending" };
                var status = statuses[random.Next(statuses.Length)];

                var order = new Order
                {
                    OrderNo = $"ORD-2025-{i:D3}",
                    UserId = user.Id,
                    Email = user.Email,
                    Total = 0m,
                    Status = status,
                    CreatedUtc = orderDate
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Add 1-4 items per order
                var itemCount = random.Next(1, 5);
                decimal total = 0m;

                for (int j = 0; j < itemCount; j++)
                {
                    var variant = variants[random.Next(variants.Count)];
                    var quantity = random.Next(1, 3);
                    var unitPrice = variant.Price;

                    db.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = 0,
                        VariantId = variant.Id,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    });

                    total += unitPrice * quantity;
                }

                order.Total = total;
                db.SaveChanges();
            }
        }
    }

    private static void CreateProductWithVariants(ApplicationDbContext db, ProductGroupData data)
    {
        // Create Product Group
        var productGroup = new ProductGroup
        {
            ObjectId = data.ObjectId,
            Name = data.Name,
            CategoryId = data.CategoryId,
            Slug = data.Slug
        };
        db.ProductGroups.Add(productGroup);
        db.SaveChanges();

        // Create Variants
        var variants = new List<Variant>();
        foreach (var variantData in data.Variants)
        {
            var variant = new Variant
            {
                Sku = variantData.Sku,
                ProductGroupId = productGroup.Id,
                Color = variantData.Color,
                Size = variantData.Size,
                Price = variantData.Price
            };
            variants.Add(variant);
        }
        db.Variants.AddRange(variants);
        db.SaveChanges();

        // Create Inventory
        for (int i = 0; i < variants.Count; i++)
        {
            db.Inventories.Add(new Inventory
            {
                VariantId = variants[i].Id,
                Quantity = data.Variants[i].Quantity,
                LowStockThreshold = data.Variants[i].LowStockThreshold
            });
        }
        db.SaveChanges();

        // Create Color Images
        foreach (var colorImageData in data.ColorImages)
        {
            db.ColorImages.Add(new ColorImage
            {
                ProductGroupId = productGroup.Id,
                Color = colorImageData.Color,
                Url = colorImageData.Url,
                IsPrimary = colorImageData.IsPrimary,
                SortOrder = colorImageData.SortOrder
            });
        }
        db.SaveChanges();

        // Create Reviews
        foreach (var reviewData in data.Reviews)
        {
            db.Reviews.Add(new Review
            {
                ProductGroupId = productGroup.Id,
                Rating = reviewData.Rating,
                Title = reviewData.Title,
                Comment = reviewData.Comment
            });
        }
        db.SaveChanges();
    }

    private static void CreatePasswordHash(string password, out string hash, out string salt)
    {
        using var hmac = new HMACSHA512();
        salt = Convert.ToBase64String(hmac.Key);
        hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    // Helper classes for structured data
    private class ProductGroupData
    {
        public string ObjectId { get; set; } = "";
        public string Name { get; set; } = "";
        public Guid CategoryId { get; set; }
        public string Slug { get; set; } = "";
        public VariantData[] Variants { get; set; } = Array.Empty<VariantData>();
        public ColorImageData[] ColorImages { get; set; } = Array.Empty<ColorImageData>();
        public ReviewData[] Reviews { get; set; } = Array.Empty<ReviewData>();
    }

    private class VariantData
    {
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }

        public VariantData(string sku, string color, string size, decimal price, int quantity, int lowStockThreshold)
        {
            Sku = sku;
            Color = color;
            Size = size;
            Price = price;
            Quantity = quantity;
            LowStockThreshold = lowStockThreshold;
        }
    }

    private class ColorImageData
    {
        public string Color { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }

        public ColorImageData(string color, string url, bool isPrimary, int sortOrder)
        {
            Color = color;
            Url = url;
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
        }
    }

    private class ReviewData
    {
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public ReviewData(int rating, string title, string comment)
        {
            Rating = rating;
            Title = title;
            Comment = comment;
        }
    }
}
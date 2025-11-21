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
            var categories = new List<Category>
            {
                new Category { Name = "Bottoms" },
                new Category { Name = "Tops" },
                new Category { Name = "Accessories" },
                new Category { Name = "Footwear" }
            };
            db.Categories.AddRange(categories);
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
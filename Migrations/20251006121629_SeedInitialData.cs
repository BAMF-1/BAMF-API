using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAMF_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "PasswordHash", "PasswordSalt", "UserName" },
                values: new object[] { 1, "hashedPassword", "salt", "admin" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentId", "Slug" },
                values: new object[] { 1, "Electronics", null, "electronics" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CreatedUtc", "Description", "IsActive", "Name", "Popularity", "Price", "Sku", "Specs", "UpdatedUtc" },
                values: new object[] { 1, "BrandX", new DateTime(2025, 10, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "High-end laptop", true, "Laptop", 10, 1499.99m, "LAP123", "{}", null });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentId", "Slug" },
                values: new object[] { 2, "Computers", 1, "computers" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce2.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Electronic devices and gadgets", true, "Electronics" },
                    { 2, "Physical and digital books", true, "Books" },
                    { 3, "Apparel and fashion items", true, "Clothing" },
                    { 4, "Home and garden supplies", true, "Home & Garden" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { "admin-001", new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), "admin@example.com", "Admin User", true, "Fxs3gMwS3/c0FmKKNjJjE6YO7WwS4M1Z5oH5n0K5K7s=", "+1234567890", "admin", null },
                    { "user-001", new DateTime(2026, 9, 15, 12, 0, 0, 0, DateTimeKind.Utc), "john@example.com", "John Doe", true, "Fxs3gMwS3/c0FmKKNjJjE6YO7WwS4M1Z5oH5n0K5K7s=", "+1987654321", "user", null },
                    { "user-002", new DateTime(2026, 9, 20, 12, 0, 0, 0, DateTimeKind.Utc), "jane@example.com", "Jane Smith", true, "Fxs3gMwS3/c0FmKKNjJjE6YO7WwS4M1Z5oH5n0K5K7s=", "+1555555555", "user", null }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "City", "Country", "IsActive", "IsDefault", "PhoneNumber", "State", "Street", "UserId", "ZipCode" },
                values: new object[,]
                {
                    { 1, "New York", "USA", true, true, "+1987654321", "NY", "123 Main Street", "user-001", "10001" },
                    { 2, "Los Angeles", "USA", true, true, "+1555555555", "CA", "456 Oak Avenue", "user-002", "90001" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CreatedAt", "IsActive", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 30, 12, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc), "user-001" },
                    { 2, new DateTime(2026, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc), "user-002" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "High-performance laptop", true, "Laptop", 999.99m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Latest smartphone model", true, "Smartphone", 699.99m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Learn C# programming", true, "C# Programming Guide", 49.99m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cotton T-shirt", true, "T-Shirt", 19.99m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Automatic coffee maker", true, "Coffee Maker", 79.99m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "AddedAt", "CartId", "Price", "ProductId", "Quantity", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 30, 12, 0, 0, 0, DateTimeKind.Utc), 1, 999.99m, 1, 1, "user-001" },
                    { 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1, 19.99m, 4, 2, "user-001" },
                    { 3, new DateTime(2026, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), 2, 699.99m, 2, 1, "user-002" }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "ProductId", "Quantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, 50, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, 100, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, 200, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 4, 500, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 5, 75, new DateTime(2026, 10, 5, 12, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "AddressId", "OrderDate", "Status", "StatusId", "TotalAmount", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 25, 12, 0, 0, 0, DateTimeKind.Utc), "delivered", 1, 1049.97m, "user-001" },
                    { 2, 2, new DateTime(2026, 9, 30, 12, 0, 0, 0, DateTimeKind.Utc), "processing", 1, 699.99m, "user-002" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "Price", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 999.99m, 1, 1 },
                    { 2, 1, 19.99m, 4, 2 },
                    { 3, 2, 699.99m, 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "admin-001");

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-001");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "user-002");
        }
    }
}

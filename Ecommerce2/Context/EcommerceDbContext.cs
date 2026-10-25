using Microsoft.EntityFrameworkCore;
using ecommerce_application.MODELS.DB_MODELS;
using ecommerce_application.MODELS;
using System;

namespace ecommerce_application.DATA
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        // DbSets for all models
        public DbSet<UserModel> Users { get; set; }
        public DbSet<AddressModel> Addresses { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<CartItemsModel> CartItems { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItemsModel> OrderItems { get; set; }
        public DbSet<PaymentsModel> Payments { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== SEED DATA ==========

            // ===== USERS SEED DATA =====
            modelBuilder.Entity<UserModel>().HasData(
                new UserModel
                {
                    Id = "admin-001",
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    PhoneNumber = "+1234567890",
                    PasswordHash = "Fxs3gMwS3/c0FmKKNjJjE6YO7WwS4M1Z5oH5n0K5K7s=", // SHA256 hash of "Admin@123"
                    Role = "admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc)
                },
                new UserModel
                {
                    Id = "user-001",
                    FullName = "John Doe",
                    Email = "john@example.com",
                    PhoneNumber = "+1987654321",
                    PasswordHash = "Fxs3gMwS3/c0FmKKNjJjE6YO7WwS4M1Z5oH5n0K5K7s=", // SHA256 hash of "Admin@123"
                    Role = "user",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 9, 15, 12, 0, 0, DateTimeKind.Utc)
                },
                new UserModel
                {
                    Id = "user-002",
                    FullName = "Jane Smith",
                    Email = "jane@example.com",
                    PhoneNumber = "+1555555555",
                    PasswordHash = "Fxs3gMwS3/c0FmKKNjJjE6YO7WwS4M1Z5oH5n0K5K7s=", // SHA256 hash of "Admin@123"
                    Role = "user",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 9, 20, 12, 0, 0, DateTimeKind.Utc)
                }
            );

            // ===== CATEGORIES SEED DATA =====
            modelBuilder.Entity<Categories>().HasData(
                new Categories { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets", IsActive = true },
                new Categories { Id = 2, Name = "Books", Description = "Physical and digital books", IsActive = true },
                new Categories { Id = 3, Name = "Clothing", Description = "Apparel and fashion items", IsActive = true },
                new Categories { Id = 4, Name = "Home & Garden", Description = "Home and garden supplies", IsActive = true }
            );

            // ===== PRODUCTS SEED DATA =====
            var seedDate = new DateTime(2026, 10, 5, 12, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Products>().HasData(
                new Products { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, CategoryId = 1, IsActive = true, Quantity = 50, CreatedAt = seedDate, UpdatedAt = seedDate, ImagePath = null },
                new Products { Id = 2, Name = "Smartphone", Description = "Latest smartphone model", Price = 699.99m, CategoryId = 1, IsActive = true, Quantity = 100, CreatedAt = seedDate, UpdatedAt = seedDate, ImagePath = null },
                new Products { Id = 3, Name = "C# Programming Guide", Description = "Learn C# programming", Price = 49.99m, CategoryId = 2, IsActive = true, Quantity = 200, CreatedAt = seedDate, UpdatedAt = seedDate, ImagePath = null },
                new Products { Id = 4, Name = "T-Shirt", Description = "Cotton T-shirt", Price = 19.99m, CategoryId = 3, IsActive = true, Quantity = 500, CreatedAt = seedDate, UpdatedAt = seedDate, ImagePath = null },
                new Products { Id = 5, Name = "Coffee Maker", Description = "Automatic coffee maker", Price = 79.99m, CategoryId = 4, IsActive = true, Quantity = 75, CreatedAt = seedDate, UpdatedAt = seedDate, ImagePath = null }
            );

            // ===== ADDRESSES SEED DATA =====
            modelBuilder.Entity<AddressModel>().HasData(
                new AddressModel
                {
                    Id = 1,
                    UserId = "user-001",
                    Street = "123 Main Street",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA",
                    PhoneNumber = "+1987654321",
                    IsDefault = true,
                    IsActive = true
                },
                new AddressModel
                {
                    Id = 2,
                    UserId = "user-002",
                    Street = "456 Oak Avenue",
                    City = "Los Angeles",
                    State = "CA",
                    ZipCode = "90001",
                    Country = "USA",
                    PhoneNumber = "+1555555555",
                    IsDefault = true,
                    IsActive = true
                }
            );

            // ===== CARTS SEED DATA =====
            modelBuilder.Entity<CartModel>().HasData(
                new CartModel
                {
                    Id = 1,
                    UserId = "user-001",
                    CreatedAt = new DateTime(2026, 9, 30, 12, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 10, 5, 12, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                },
                new CartModel
                {
                    Id = 2,
                    UserId = "user-002",
                    CreatedAt = new DateTime(2026, 10, 2, 12, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 10, 5, 12, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                }
            );

            // ===== CART ITEMS SEED DATA =====
            modelBuilder.Entity<CartItemsModel>().HasData(
                new CartItemsModel
                {
                    Id = 1,
                    UserId = "user-001",
                    CartId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    Price = 999.99m,
                    AddedAt = new DateTime(2026, 9, 30, 12, 0, 0, DateTimeKind.Utc)
                },
                new CartItemsModel
                {
                    Id = 2,
                    UserId = "user-001",
                    CartId = 1,
                    ProductId = 4,
                    Quantity = 2,
                    Price = 19.99m,
                    AddedAt = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new CartItemsModel
                {
                    Id = 3,
                    UserId = "user-002",
                    CartId = 2,
                    ProductId = 2,
                    Quantity = 1,
                    Price = 699.99m,
                    AddedAt = new DateTime(2026, 10, 2, 12, 0, 0, DateTimeKind.Utc)
                }
            );

            // ===== ORDERS SEED DATA =====
            modelBuilder.Entity<Orders>().HasData(
                new Orders
                {
                    Id = 1,
                    UserId = "user-001",
                    AddressId = 1,
                    OrderDate = new DateTime(2026, 9, 25, 12, 0, 0, DateTimeKind.Utc),
                    TotalAmount = 1049.97m,
                    Status = "delivered",
                    StatusId = 1
                },
                new Orders
                {
                    Id = 2,
                    UserId = "user-002",
                    AddressId = 2,
                    OrderDate = new DateTime(2026, 9, 30, 12, 0, 0, DateTimeKind.Utc),
                    TotalAmount = 699.99m,
                    Status = "processing",
                    StatusId = 1
                }
            );

            // ===== ORDER ITEMS SEED DATA =====
            modelBuilder.Entity<OrderItemsModel>().HasData(
                new OrderItemsModel
                {
                    Id = 1,
                    UserId = "user-001",
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    Price = 999.99m
                },
                new OrderItemsModel
                {
                    Id = 2,
                    UserId = "user-001",
                    OrderId = 1,
                    ProductId = 4,
                    Quantity = 2,
                    Price = 19.99m
                },
                new OrderItemsModel
                {
                    Id = 3,
                    UserId = "user-002",
                    OrderId = 2,
                    ProductId = 2,
                    Quantity = 1,
                    Price = 699.99m
                }
            );

            // ========== RELATIONSHIPS CONFIGURATION ==========

            // ===== USER - ADDRESS (One-to-Many) =====
            modelBuilder.Entity<AddressModel>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)  // ✅ Added navigation from User
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Addresses_Users");

            // ===== USER - CART (One-to-Many) =====
            modelBuilder.Entity<CartModel>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)  // ✅ Added navigation from User
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Carts_Users");

            // ===== USER - ORDERS (One-to-Many) =====
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)  // ✅ Added relationship
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_Users");


            // ===== USER - CARTITEMS (One-to-Many) =====
            modelBuilder.Entity<CartItemsModel>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CartItems_Users");

            // ===== PRODUCT - CARTITEMS (One-to-Many) =====
            modelBuilder.Entity<CartItemsModel>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CartItems_Products");

            // ===== USER - CATEGORIES (One-to-Many) =====
            modelBuilder.Entity<Categories>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Categories_Users");

            // ===== USER - PRODUCTS (One-to-Many) =====
            modelBuilder.Entity<Products>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Products_Users");

            // ===== CATEGORY - PRODUCTS (One-to-Many) =====
            modelBuilder.Entity<Products>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Products_Categories");

            // ===== ADDRESS - ORDERS (One-to-Many) =====
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_Addresses");

            // ===== ORDERS - ORDERITEMS (One-to-Many) =====
            modelBuilder.Entity<OrderItemsModel>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItems_Orders");

            // ===== USER - ORDERITEMS (One-to-Many) =====
            modelBuilder.Entity<OrderItemsModel>()
                .HasOne(oi => oi.User)
                .WithMany()
                .HasForeignKey(oi => oi.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItems_Users");

            // ===== PRODUCT - ORDERITEMS (One-to-Many) =====
            modelBuilder.Entity<OrderItemsModel>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_OrderItems_Products");

            // ===== ORDERS - PAYMENTS (One-to-Many) =====
            modelBuilder.Entity<PaymentsModel>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payments_Orders");

            // ===== USER - REFRESHTOKEN (One-to-Many) =====
            modelBuilder.Entity<RefreshTokenModel>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)  // ✅ Added navigation from User
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RefreshTokens_Users");

            // ========== INDEXES FOR PERFORMANCE ==========

            // User indexes
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email_Unique");

            // Product indexes
            modelBuilder.Entity<Products>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            modelBuilder.Entity<Products>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            modelBuilder.Entity<Products>()
                .HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            // Category indexes
            modelBuilder.Entity<Categories>()
                .HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Name_Unique");

            // RefreshToken indexes
            modelBuilder.Entity<RefreshTokenModel>()
                .HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token_Unique");

            // Order indexes
            modelBuilder.Entity<Orders>()
                .HasIndex(o => o.UserId)
                .HasDatabaseName("IX_Orders_UserId");

            // Cart indexes
            modelBuilder.Entity<CartModel>()
                .HasIndex(c => c.UserId)
                .HasDatabaseName("IX_Carts_UserId");
        }
    }
}
# Step-by-Step: Extract Product Service (First Microservice)

## Prerequisites
- SQL Server instance running
- Docker Desktop (for containerization)
- .NET 8 SDK installed

---

## Step 1: Create Database for Product Service

```sql
-- Run on your SQL Server
CREATE DATABASE Ecommerce_Product;

USE Ecommerce_Product;

-- Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    UserId NVARCHAR(450),
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Categories_Users FOREIGN KEY (UserId) 
        REFERENCES dbo.Users(Id) ON DELETE CASCADE
);

-- Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT DEFAULT 0,
    CategoryId INT NOT NULL,
    UserId NVARCHAR(450),
    IsActive BIT DEFAULT 1,
    ImagePath NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) 
        REFERENCES Categories(Id),
    CONSTRAINT FK_Products_Users FOREIGN KEY (UserId) 
        REFERENCES dbo.Users(Id)
);

-- Inventory Table (Optional, for better tracking)
CREATE TABLE Inventory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL UNIQUE,
    TotalQuantity INT DEFAULT 0,
    ReservedQuantity INT DEFAULT 0,
    AvailableQuantity INT DEFAULT 0,
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (ProductId) 
        REFERENCES Products(Id) ON DELETE CASCADE
);

-- Indexes for Performance
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Products_Name ON Products(Name);
CREATE INDEX IX_Categories_Name ON Categories(Name);
```

---

## Step 2: Create Product Service Solution

### Directory Structure
```
ProductService/
├── ProductService.API/
│   ├── Controllers/
│   │   └── ProductsController.cs
│   │   └── CategoriesController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── ProductService.BLL/
│   ├── Services/
│   │   ├── IProductService.cs
│   │   ├── ProductService.cs
│   │   ├── ICategoryService.cs
│   │   └── CategoryService.cs
│   └── DTOs/
│       ├── ProductDto.cs
│       └── CategoryDto.cs
├── ProductService.DAL/
│   ├── Data/
│   │   └── ProductDbContext.cs
│   ├── Repositories/
│   │   ├── IProductRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   └── CategoryRepository.cs
│   └── Migrations/
└── ProductService.Models/
    ├── Entities/
    │   ├── Product.cs
    │   ├── Category.cs
    │   └── Inventory.cs
    └── Responses/
        └── ApiResponse.cs
```

### Create Projects
```powershell
cd ProductService

dotnet new sln -n ProductService
dotnet new classlib -n ProductService.Models
dotnet new classlib -n ProductService.BLL
dotnet new classlib -n ProductService.DAL
dotnet new webapi -n ProductService.API

dotnet sln add ProductService.Models
dotnet sln add ProductService.BLL
dotnet sln add ProductService.DAL
dotnet sln add ProductService.API

# Add dependencies
cd ProductService.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Serilog
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add reference ../ProductService.DAL/ProductService.DAL.csproj
dotnet add reference ../ProductService.BLL/ProductService.BLL.csproj
```

---

## Step 3: Create Models (ProductService.Models)

### Product.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string UserId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public string ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Inventory Inventory { get; set; }
    }
}
```

### Category.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace ProductService.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
```

### Inventory.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Models.Entities
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int TotalQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

### DTOs
```csharp
// ProductDto.cs
namespace ProductService.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
    }
}

// CategoryDto.cs
namespace ProductService.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
```

---

## Step 4: Create DbContext (ProductService.DAL)

### ProductDbContext.cs
```csharp
using Microsoft.EntityFrameworkCore;
using ProductService.Models.Entities;

namespace ProductService.DAL.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category - Product relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Products_Categories");

            // Product - Inventory relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Name_Unique");

            // Seed Data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices", IsActive = true },
                new Category { Id = 2, Name = "Books", Description = "Physical and digital books", IsActive = true },
                new Category { Id = 3, Name = "Clothing", Description = "Apparel items", IsActive = true },
                new Category { Id = 4, Name = "Home & Garden", Description = "Home supplies", IsActive = true }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "High-performance laptop",
                    Price = 999.99m,
                    Quantity = 50,
                    CategoryId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Description = "Latest smartphone",
                    Price = 699.99m,
                    Quantity = 100,
                    CategoryId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
```

---

## Step 5: Create Repositories (ProductService.DAL)

### IProductRepository.cs
```csharp
using ProductService.Models.Entities;

namespace ProductService.DAL.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetByCategoryAsync(int categoryId);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
```

### ProductRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using ProductService.DAL.Data;
using ProductService.Models.Entities;

namespace ProductService.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
```

### Category Repository (Similar Pattern)
```csharp
using Microsoft.EntityFrameworkCore;
using ProductService.DAL.Data;
using ProductService.Models.Entities;

namespace ProductService.DAL.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<List<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductDbContext _context;

        public CategoryRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsActive = false;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

---

## Step 6: Create Services (ProductService.BLL)

### IProductService.cs & ProductService.cs
```csharp
using ProductService.Models.DTOs;

namespace ProductService.BLL.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductQuantityAsync(int productId, int quantityToDecrement);
    }
}

// Implementation
using ProductService.DAL.Repositories;
using ProductService.Models.DTOs;
using ProductService.Models.Entities;

namespace ProductService.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return MapToDto(product);
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _productRepository.CreateAsync(product);
            return MapToDto(created);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;

            var updated = await _productRepository.UpdateAsync(product);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateProductQuantityAsync(int productId, int quantityToDecrement)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.Quantity < quantityToDecrement)
                return false;

            product.Quantity -= quantityToDecrement;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }
    }
}
```

---

## Step 7: Create Controllers (ProductService.API)

### ProductsController.cs
```csharp
using Microsoft.AspNetCore.Mvc;
using ProductService.BLL.Services;
using ProductService.Models.DTOs;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
                return NotFound("Product not found");

            return NoContent();
        }

        [HttpPost("{id}/reduce-quantity")]
        public async Task<ActionResult> ReduceProductQuantity(int id, [FromBody] int quantity)
        {
            var success = await _productService.UpdateProductQuantityAsync(id, quantity);
            if (!success)
                return BadRequest("Insufficient quantity or product not found");

            return Ok("Quantity reduced successfully");
        }
    }
}
```

---

## Step 8: Configure Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Serilog;
using ProductService.DAL.Data;
using ProductService.DAL.Repositories;
using ProductService.BLL.Services;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "logs/productservice-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Log.Information("Product Service started");
app.Run();
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Ecommerce_Product;User Id=sa;Password=YourPassword123;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

---

## Step 9: Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProductService.API/ProductService.API.csproj", "ProductService.API/"]
COPY ["ProductService.BLL/ProductService.BLL.csproj", "ProductService.BLL/"]
COPY ["ProductService.DAL/ProductService.DAL.csproj", "ProductService.DAL/"]
COPY ["ProductService.Models/ProductService.Models.csproj", "ProductService.Models/"]

RUN dotnet restore "ProductService.API/ProductService.API.csproj"

COPY . .
RUN dotnet build "ProductService.API/ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.API/ProductService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5002
ENTRYPOINT ["dotnet", "ProductService.API.dll"]
```

---

## Step 10: Create & Run Migrations

```powershell
cd ProductService.API

# Create migration
dotnet ef migrations add InitialCreate -p ../ProductService.DAL -c ProductDbContext

# Apply migration
dotnet ef database update -p ../ProductService.DAL -c ProductDbContext
```

---

## Step 11: Docker Compose for Local Testing

### docker-compose.yml
```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourPassword123
      MSSQL_PID: Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql

  product-service:
    build:
      context: .
      dockerfile: ProductService.API/Dockerfile
    ports:
      - "5002:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=Ecommerce_Product;User Id=sa;Password=YourPassword123;Encrypt=false;
    depends_on:
      - sqlserver
    volumes:
      - ./ProductService.API/logs:/app/logs

volumes:
  sqlserver-data:
```

---

## Step 12: Run & Test

```powershell
# Run locally without Docker
cd ProductService.API
dotnet run

# Or with Docker Compose
docker-compose up -d

# Test endpoints
curl http://localhost:5002/api/products
curl http://localhost:5002/api/products/1
```

---

## Migration Checklist

- [ ] Database created (`Ecommerce_Product`)
- [ ] Solution structure created
- [ ] Models defined
- [ ] DbContext configured
- [ ] Repositories implemented
- [ ] Services implemented
- [ ] Controllers created
- [ ] Program.cs configured
- [ ] Migrations applied
- [ ] Dockerfile created
- [ ] Docker Compose tested locally
- [ ] API endpoints tested with Postman/curl
- [ ] Ready to integrate with API Gateway

---

## Next: Repeat for Other Services
1. **Auth Service** (extract User/Auth tables)
2. **Cart Service** (depends on Product Service)
3. **Order Service** (depends on Cart, Product, Auth)
4. **Payment Service** (depends on Order Service)


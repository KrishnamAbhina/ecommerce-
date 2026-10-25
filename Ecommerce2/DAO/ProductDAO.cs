using DAL.DTO;
using ecommerce_application.MODELS;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace DAL.Services
{
    public class ProductDAO
    {
        private readonly EcommerceDbContext _context;
        private readonly ILogger<ProductDAO> _logger;

        public ProductDAO(EcommerceDbContext context, ILogger<ProductDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ============ HELPER METHODS ============

        /// <summary>
        /// Validates product ID existence
        /// </summary>
        private async Task<Products> CheckValidIdExistAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Product ID must be  than 0.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found.");
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            return product;
        }

        /// <summary>
        /// Validates category ID existence
        /// </summary>
        private async Task<Categories> CheckValidCategoryExistAsync(int categoryId)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.");
            }

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {categoryId} not found.");
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            return category;
        }

        // ============ CREATE ============

        /// <summary>
        /// Create a new product
        /// </summary>
        public async Task<int> CreateProductAsync(CreateProductVM model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Product data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    throw new ArgumentException("Product name is required.");
                }

                if (model.Price <= 0)
                {
                    throw new ArgumentException("Product price must be greater than 0.");
                }

                if (model.CategoryId <= 0)
                {
                    throw new ArgumentException("Valid category ID is required.");
                }

                // Validate category exists
                await CheckValidCategoryExistAsync(model.CategoryId);

                // Map DTO to Entity
                var product = new Products
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ImagePath = model.ImagePath?.Trim(),
                    Quantity = model.Quantity,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product '{product.Name}' created successfully with ID {product.Id}.");
                return product.Id;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database error creating product: {dbEx.InnerException?.Message}");
                throw new Exception($"Database error: {dbEx.InnerException?.Message}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating product: {ex.Message}");
                throw;
            }
        }

        // ============ READ ============

        /// <summary>
        /// Get all products
        /// </summary>
        public async Task<List<ProductVM>> GetAllProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} products.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all products: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all products for specific user
        /// </summary>
        public async Task<List<ProductVM>> GetAllProductsAsync(string userId)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.UserId == userId)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} products for user {userId}.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all products for user {userId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public async Task<ProductVM> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                return new ProductVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category != null ? product.Category.Name : "Unknown",
                    IsActive = product.IsActive,
                    UserId = product.UserId,
                    ImagePath = product.ImagePath,
                    Quantity = product.Quantity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product by ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get product by ID for specific user
        /// </summary>
        public async Task<ProductVM> GetProductByIdAsync(string userId, int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found for user {userId}.");
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                return new ProductVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category != null ? product.Category.Name : "Unknown",
                    IsActive = product.IsActive,
                    UserId = product.UserId,
                    ImagePath = product.ImagePath,
                    Quantity = product.Quantity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product by ID {id} for user {userId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all active products
        /// </summary>
        public async Task<List<ProductVM>> GetActiveProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} active products.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active products: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all active products for specific user
        /// </summary>
        public async Task<List<ProductVM>> GetActiveProductsAsync(string userId)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive && p.UserId == userId)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} active products for user {userId}.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active products for user {userId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get products by category ID
        /// </summary>
        public async Task<List<ProductVM>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than 0.");
                }

                // Validate category exists
                await CheckValidCategoryExistAsync(categoryId);

                var products = await _context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} products from category {categoryId}.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving products by category {categoryId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get products by category ID for specific user
        /// </summary>
        public async Task<List<ProductVM>> GetProductsByCategoryAsync(string userId, int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than 0.");
                }

                // Validate category exists
                await CheckValidCategoryExistAsync(categoryId);

                var products = await _context.Products
                    .Where(p => p.CategoryId == categoryId && p.UserId == userId)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} products from category {categoryId} for user {userId}.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving products by category {categoryId} for user {userId}: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Search products by name or description
        /// </summary>
        public async Task<List<ProductVM>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    throw new ArgumentException("Search term cannot be empty.");
                }

                var term = searchTerm.ToLower().Trim();

                var products = await _context.Products
                    .Where(p => p.Name.ToLower().Contains(term) ||
                                p.Description.ToLower().Contains(term))
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Search for '{searchTerm}' returned {products.Count} results.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching products with term '{searchTerm}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Search products by name or description for specific user
        /// </summary>
        public async Task<List<ProductVM>> SearchProductsAsync(string userId, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    throw new ArgumentException("Search term cannot be empty.");
                }

                var term = searchTerm.ToLower().Trim();

                var products = await _context.Products
                    .Where(p => (p.Name.ToLower().Contains(term) ||
                                p.Description.ToLower().Contains(term)) && p.UserId == userId)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive,
                        UserId = p.UserId,
                        ImagePath = p.ImagePath,
                        Quantity = p.Quantity
                    })
                    .ToListAsync();

                _logger.LogInformation($"Search for '{searchTerm}' for user {userId} returned {products.Count} results.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching products with term '{searchTerm}' for user {userId}: {ex.Message}");
                throw;
            }
        }

        // ============ UPDATE ============

        /// <summary>
        /// Update an existing product
        /// </summary>
        public async Task<bool> UpdateProductAsync(int id, UpdateProductVM model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Product data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    throw new ArgumentException("Product name is required.");
                }

                if (model.Price <= 0)
                {
                    throw new ArgumentException("Product price must be greater than 0.");
                }

                if (model.CategoryId <= 0)
                {
                    throw new ArgumentException("Valid category ID is required.");
                }

                // Validate product exists
                var product = await CheckValidIdExistAsync(id);

                // Validate category exists
                await CheckValidCategoryExistAsync(model.CategoryId);

                // Update fields
                product.Name = model.Name.Trim();
                product.Description = model.Description?.Trim();
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.ImagePath = model.ImagePath?.Trim();
                product.Quantity = model.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update an existing product for specific user
        /// </summary>
        public async Task<bool> UpdateProductAsync(string userId, int id, UpdateProductVM model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Product data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    throw new ArgumentException("Product name is required.");
                }

                if (model.Price <= 0)
                {
                    throw new ArgumentException("Product price must be greater than 0.");
                }

                if (model.CategoryId <= 0)
                {
                    throw new ArgumentException("Valid category ID is required.");
                }

                // Validate product exists and belongs to user
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                // Validate category exists
                await CheckValidCategoryExistAsync(model.CategoryId);

                // Update fields
                product.Name = model.Name.Trim();
                product.Description = model.Description?.Trim();
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.ImagePath = model.ImagePath?.Trim();
                product.Quantity = model.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} updated successfully for user {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {id} for user {userId}: {ex.Message}");
                return false;
            }
        }

        // ============ DELETE ============

        /// <summary>
        /// Delete a product
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await CheckValidIdExistAsync(id);

                // Find and remove related cart items
                var cartItems = await _context.CartItems.Where(ci => ci.ProductId == id).ToListAsync();
                var cartIdsToCheck = cartItems.Select(ci => ci.CartId).Distinct().ToList();
                _context.CartItems.RemoveRange(cartItems);

                // Delete empty carts
                foreach (var cartId in cartIdsToCheck)
                {
                    var cartItemCount = await _context.CartItems.CountAsync(ci => ci.CartId == cartId);
                    if (cartItemCount == 0)
                    {
                        var cart = await _context.Carts.FindAsync(cartId);
                        if (cart != null)
                        {
                            _context.Carts.Remove(cart);
                            _logger.LogInformation($"Deleted empty cart with ID {cartId}.");
                        }
                    }
                }

                // Find and remove related order items
                var orderItems = await _context.OrderItems.Where(oi => oi.ProductId == id).ToListAsync();
                var orderIdsToCheck = orderItems.Select(oi => oi.OrderId).Distinct().ToList();
                _context.OrderItems.RemoveRange(orderItems);

                // Delete empty orders
                foreach (var orderId in orderIdsToCheck)
                {
                    var orderItemCount = await _context.OrderItems.CountAsync(oi => oi.OrderId == orderId);
                    if (orderItemCount == 0)
                    {
                        var order = await _context.Orders.FindAsync(orderId);
                        if (order != null)
                        {
                            _context.Orders.Remove(order);
                            _logger.LogInformation($"Deleted empty order with ID {orderId}.");
                        }
                    }
                }

                // Remove the product
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} and all related empty carts/orders deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a product for specific user
        /// </summary>
        public async Task<bool> DeleteProductAsync(string userId, int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                // Find and remove related cart items
                var cartItems = await _context.CartItems.Where(ci => ci.ProductId == id).ToListAsync();
                var cartIdsToCheck = cartItems.Select(ci => ci.CartId).Distinct().ToList();
                _context.CartItems.RemoveRange(cartItems);

                // Delete empty carts
                foreach (var cartId in cartIdsToCheck)
                {
                    var cartItemCount = await _context.CartItems.CountAsync(ci => ci.CartId == cartId);
                    if (cartItemCount == 0)
                    {
                        var cart = await _context.Carts.FindAsync(cartId);
                        if (cart != null)
                        {
                            _context.Carts.Remove(cart);
                            _logger.LogInformation($"Deleted empty cart with ID {cartId}.");
                        }
                    }
                }

                // Find and remove related order items
                var orderItems = await _context.OrderItems.Where(oi => oi.ProductId == id).ToListAsync();
                var orderIdsToCheck = orderItems.Select(oi => oi.OrderId).Distinct().ToList();
                _context.OrderItems.RemoveRange(orderItems);

                // Delete empty orders
                foreach (var orderId in orderIdsToCheck)
                {
                    var orderItemCount = await _context.OrderItems.CountAsync(oi => oi.OrderId == orderId);
                    if (orderItemCount == 0)
                    {
                        var order = await _context.Orders.FindAsync(orderId);
                        if (order != null)
                        {
                            _context.Orders.Remove(order);
                            _logger.LogInformation($"Deleted empty order with ID {orderId}.");
                        }
                    }
                }

                // Remove the product
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} and all related empty carts/orders deleted successfully for user {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id} for user {userId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deactivate a product
        /// </summary>
        public async Task<bool> DeactivateProductAsync(int id)
        {
            try
            {
                var product = await CheckValidIdExistAsync(id);

                product.IsActive = false;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} deactivated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating product with ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deactivate a product for specific user
        /// </summary>
        public async Task<bool> DeactivateProductAsync(string userId, int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                product.IsActive = false;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} deactivated successfully for user {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating product with ID {id} for user {userId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Activate a product
        /// </summary>
        public async Task<bool> ActivateProductAsync(int id)
        {
            try
            {
                var product = await CheckValidIdExistAsync(id);

                product.IsActive = true;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} activated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating product with ID {id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Activate a product for specific user
        /// </summary>
        public async Task<bool> ActivateProductAsync(string userId, int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                product.IsActive = true;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product with ID {id} activated successfully for user {userId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating product with ID {id} for user {userId}: {ex.Message}");
                return false;
            }
        }

        // ============ ADDITIONAL METHODS ============

        /// <summary>
        /// Get products by category with count
        /// </summary>
        public async Task<List<ProductVM>> GetProductsByCategoryWithCountAsync(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than 0.");
                }

                // Validate category exists
                await CheckValidCategoryExistAsync(categoryId);

                var products = await _context.Products
                    .Where(p => p.CategoryId == categoryId && p.IsActive)
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} active products from category {categoryId}.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving products by category {categoryId}: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Get product statistics (uses Inventory table for stock data)
        /// </summary>
        public async Task<ProductStatisticsVM> GetProductStatisticsAsync()
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync();
                var activeProducts = await _context.Products.Where(p => p.IsActive).CountAsync();
                var outOfStockProducts = await _context.Products.CountAsync(p => p.Quantity == 0);
                var totalValue = await _context.Products
                    .SumAsync(p => p.Price * p.Quantity);

                return new ProductStatisticsVM
                {
                    TotalProducts = totalProducts,
                    ActiveProducts = activeProducts,
                    InactiveProducts = totalProducts - activeProducts,
                    OutOfStockProducts = outOfStockProducts,
                    TotalInventoryValue = totalValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product statistics: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all products with category details
        /// </summary>
        public async Task<List<ProductVM>> GetAllProductsWithCategoryAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new ProductVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown",
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} products with category details.");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving products with category: {ex.Message}");
                throw;
            }
        }
    }
}
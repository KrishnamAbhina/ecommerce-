
using DAL.DTO;
using ecommerce_application.MODELS;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class CategoryDAO
    {
        private readonly EcommerceDbContext _context;
        private readonly ILogger<CategoryDAO> _logger;

        public CategoryDAO(EcommerceDbContext context, ILogger<CategoryDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ============ HELPER METHOD ============

        /// <summary>
        /// Validates category ID existence
        /// </summary>
        private async Task<Categories> CheckValidIdExistAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

        // ============ CREATE ============

        /// <summary>
        /// Create a new category
        /// </summary>
        public async Task<int> CreateCategoryAsync(CreateCategoryVM model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Category data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    throw new ArgumentException("Category name is required.");
                }

                // Check if category name already exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower());

                if (existingCategory != null)
                {
                    throw new InvalidOperationException($"A category with name '{model.Name}' already exists.");
                }

                // Map DTO to Entity
                var category = new Categories
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    IsActive = true,
                    UserId = null
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category '{category.Name}' created successfully with ID {category.Id}.");
                return category.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating category: {ex.Message}");
                throw;
            }
        }

        // ============ READ ============

        /// <summary>
        /// Get all categories
        /// </summary>
        public async Task<List<CategoryVM>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .OrderByDescending(c => c.Id)
                    .Select(c => new CategoryVM
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {categories.Count} categories.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all categories: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all categories with their products
        /// </summary>
        public async Task<List<CategoryWithProductsVM>> GetAllCategoriesWithProductsAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .OrderByDescending(c => c.Id)
                    .Select(c => new CategoryWithProductsVM
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        Products = c.Products
                            .Where(p => p.IsActive)
                            .Select(p => new ProductInCategoryVM
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                IsActive = p.IsActive
                            })
                            .ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {categories.Count} categories with products.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all categories with products: {ex.Message}");
                throw;
            }
        }

        /// <summary>

        /// <summary>
        /// Get category by ID with products
        /// </summary>
        public async Task<CategoryWithProductsVM> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }

                return new CategoryWithProductsVM
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    Products = category.Products
                        .Where(p => p.IsActive)
                        .Select(p => new ProductInCategoryVM
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            IsActive = p.IsActive
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving category by ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all active categories
        /// </summary>
        public async Task<List<CategoryVM>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.Id)
                    .Select(c => new CategoryVM
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {categories.Count} active categories.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active categories: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Get category by name
        /// </summary>
        public async Task<CategoryVM> GetCategoryByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Category name is required.");
                }

                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

                if (category == null)
                {
                    _logger.LogWarning($"Category with name '{name}' not found.");
                    throw new KeyNotFoundException($"Category with name '{name}' not found.");
                }

                return new CategoryVM
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving category by name '{name}': {ex.Message}");
                throw;
            }
        }


        // ============ UPDATE ============

        /// <summary>
        /// Update an existing category
        /// </summary>
        public async Task UpdateCategoryAsync(int id, UpdateCategoryVM model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Category data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    throw new ArgumentException("Category name is required.");
                }

                // Validate category exists
                var category = await CheckValidIdExistAsync(id);

                // Check if new name already exists (excluding current category)
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.Id != id);

                if (existingCategory != null)
                {
                    throw new InvalidOperationException($"A category with name '{model.Name}' already exists.");
                }

                // Update fields
                category.Name = model.Name.Trim();
                category.Description = model.Description?.Trim();

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with ID {id}: {ex.Message}");
                throw;
            }
        }


        // ============ DELETE ============

        /// <summary>
        /// Delete a category
        /// </summary>
        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await CheckValidIdExistAsync(id);

                // Check if category has products
                var productCount = await _context.Products
                    .CountAsync(p => p.CategoryId == id);

                if (productCount > 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot delete category '{category.Name}'. It contains {productCount} product(s). " +
                        $"Please move or delete the products first.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with ID {id}: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Deactivate a category
        /// </summary>
        public async Task DeactivateCategoryAsync(int id)
        {
            try
            {
                var category = await CheckValidIdExistAsync(id);

                category.IsActive = false;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category with ID {id} deactivated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating category with ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>

        /// <summary>
        /// Activate a category
        /// </summary>
        public async Task ActivateCategoryAsync(int id)
        {
            try
            {
                var category = await CheckValidIdExistAsync(id);

                category.IsActive = true;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category with ID {id} activated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating category with ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>

        // ============ ADDITIONAL METHODS ============

        /// <summary>
        /// Get category with product count
        /// </summary>
        public async Task<CategoryWithProductCountVM> GetCategoryWithProductCountAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    throw new KeyNotFoundException($"Category with ID {id} not found.");
                }

                var productCount = await _context.Products
                    .CountAsync(p => p.CategoryId == id && p.IsActive);

                return new CategoryWithProductCountVM
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    ProductCount = productCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving category with product count for ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>

        /// <summary>
        /// Get all categories with product count
        /// </summary>
        public async Task<List<CategoryWithProductCountVM>> GetAllCategoriesWithProductCountAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .OrderByDescending(c => c.Id)
                    .Select(c => new CategoryWithProductCountVM
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        ProductCount = _context.Products
                            .Count(p => p.CategoryId == c.Id && p.IsActive)
                        
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {categories.Count} categories with product counts.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving categories with product counts: {ex.Message}");
                throw;
            }
        }
    }
}
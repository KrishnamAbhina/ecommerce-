using DAL.DTO;
using DAL.Services;
using ecommerce_application.MODELS.DB_MODELS;
using ecommerce_application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_application.SERVICES
{
    public interface IProductService
    {
        Task<int> AddProductAsync(CreateProductVM model);
        Task<int> AddProductAsync(string userId, CreateProductVM model);
        Task<List<ProductVM>> GetAllProductsAsync();
        Task<List<ProductVM>> GetAllProductsAsync(string userId);
        Task<ProductVM> GetProductByIdAsync(int id);
        Task<ProductVM> GetProductByIdAsync(string userId, int id);
        Task<List<ProductVM>> GetActiveProductsAsync();
        Task<List<ProductVM>> GetActiveProductsAsync(string userId);
        Task<List<ProductVM>> GetProductsByCategoryAsync(int categoryId);
        Task<List<ProductVM>> GetProductsByCategoryAsync(string userId, int categoryId);
        Task<List<ProductVM>> SearchProductsAsync(string searchTerm);
        Task<List<ProductVM>> SearchProductsAsync(string userId, string searchTerm);
        Task<bool> UpdateProductAsync(int id, UpdateProductVM model);
        Task<bool> UpdateProductAsync(string userId, int id, UpdateProductVM model);
        Task<bool> UpdateProductAsync(Products product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> DeleteProductAsync(string userId, int id);
        Task<bool> DeactivateProductAsync(int id);
        Task<bool> DeactivateProductAsync(string userId, int id);
        Task<bool> ActivateProductAsync(int id);
        Task<bool> ActivateProductAsync(string userId, int id);
        Task<ProductStatisticsVM> GetProductStatisticsAsync();
        Task<List<ProductVM>> GetProductsByCategoryWithCountAsync(int categoryId);
    }

    public class ProductService : IProductService
    {
        private readonly ProductDAO _productDAO;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductDAO productDAO, ILogger<ProductService> logger)
        {
            _productDAO = productDAO;
            _logger = logger;
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        public async Task<int> AddProductAsync(CreateProductVM model)
        {
            try
            {
                _logger.LogInformation("ProductService: AddProductAsync called");

                // Handle image upload if provided
                if (model.Image != null)
                {
                    var imagePath = await FileUploadHelper.UploadProductImageAsync(model.Image);
                    model.ImagePath = imagePath;
                }

                return await _productDAO.CreateProductAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in AddProductAsync");
                throw;
            }
        }

        /// <summary>
        /// Add a new product for specific user
        /// </summary>
        public async Task<int> AddProductAsync(string userId, CreateProductVM model)
        {
            try
            {
                _logger.LogInformation($"ProductService: AddProductAsync called for userId {userId}");

                // Handle image upload if provided
                if (model.Image != null)
                {
                    var imagePath = await FileUploadHelper.UploadProductImageAsync(model.Image);
                    model.ImagePath = imagePath;
                }

                return await _productDAO.CreateProductAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in AddProductAsync");
                throw;
            }
        }

        /// <summary>
        /// Get all products
        /// </summary>
        public async Task<List<ProductVM>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("ProductService: GetAllProductsAsync called");
                return await _productDAO.GetAllProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in GetAllProductsAsync");
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
                _logger.LogInformation($"ProductService: GetAllProductsAsync called for userId {userId}");
                return await _productDAO.GetAllProductsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in GetAllProductsAsync");
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
                _logger.LogInformation($"ProductService: GetProductByIdAsync called with ID {id}");
                return await _productDAO.GetProductByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in GetProductByIdAsync for ID {id}");
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
                _logger.LogInformation($"ProductService: GetProductByIdAsync called with userId {userId} and ID {id}");
                return await _productDAO.GetProductByIdAsync(userId, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in GetProductByIdAsync for ID {id}");
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
                _logger.LogInformation("ProductService: GetActiveProductsAsync called");
                return await _productDAO.GetActiveProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in GetActiveProductsAsync");
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
                _logger.LogInformation($"ProductService: GetActiveProductsAsync called for userId {userId}");
                return await _productDAO.GetActiveProductsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in GetActiveProductsAsync");
                throw;
            }
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        public async Task<List<ProductVM>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation($"ProductService: GetProductsByCategoryAsync called with category {categoryId}");
                return await _productDAO.GetProductsByCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in GetProductsByCategoryAsync for category {categoryId}");
                throw;
            }
        }

        /// <summary>
        /// Get products by category for specific user
        /// </summary>
        public async Task<List<ProductVM>> GetProductsByCategoryAsync(string userId, int categoryId)
        {
            try
            {
                _logger.LogInformation($"ProductService: GetProductsByCategoryAsync called with userId {userId} and category {categoryId}");
                return await _productDAO.GetProductsByCategoryAsync(userId, categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in GetProductsByCategoryAsync for category {categoryId}");
                throw;
            }
        }

        /// <summary>
        /// Search products
        /// </summary>
        public async Task<List<ProductVM>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation($"ProductService: SearchProductsAsync called with term '{searchTerm}'");
                return await _productDAO.SearchProductsAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in SearchProductsAsync for term '{searchTerm}'");
                throw;
            }
        }

        /// <summary>
        /// Search products for specific user
        /// </summary>
        public async Task<List<ProductVM>> SearchProductsAsync(string userId, string searchTerm)
        {
            try
            {
                _logger.LogInformation($"ProductService: SearchProductsAsync called for userId {userId} with term '{searchTerm}'");
                return await _productDAO.SearchProductsAsync(userId, searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in SearchProductsAsync for term '{searchTerm}'");
                throw;
            }
        }

        /// <summary>
        /// Update product
        /// </summary>
        public async Task<bool> UpdateProductAsync(int id, UpdateProductVM model)
        {
            try
            {
                _logger.LogInformation($"ProductService: UpdateProductAsync called for ID {id}");

                // Handle image upload if provided
                if (model.Image != null && model.Image.Length > 0)
                {
                    string imagePath = await FileUploadHelper.UploadProductImageAsync(model.Image);
                    model.ImagePath = imagePath;
                }

                return await _productDAO.UpdateProductAsync(id, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in UpdateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Update product for specific user
        /// </summary>
        public async Task<bool> UpdateProductAsync(string userId, int id, UpdateProductVM model)
        {
            try
            {
                _logger.LogInformation($"ProductService: UpdateProductAsync called for userId {userId} and ID {id}");

                // Handle image upload if provided
                if (model.Image != null)
                {
                    var imagePath = await FileUploadHelper.UploadProductImageAsync(model.Image);
                    model.ImagePath = imagePath;
                }

                return await _productDAO.UpdateProductAsync(userId, id, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in UpdateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Delete product
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation($"ProductService: DeleteProductAsync called for ID {id}");
                return await _productDAO.DeleteProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in DeleteProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Delete product for specific user
        /// </summary>
        public async Task<bool> DeleteProductAsync(string userId, int id)
        {
            try
            {
                _logger.LogInformation($"ProductService: DeleteProductAsync called for userId {userId} and ID {id}");
                return await _productDAO.DeleteProductAsync(userId, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in DeleteProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Deactivate product
        /// </summary>
        public async Task<bool> DeactivateProductAsync(int id)
        {
            try
            {
                _logger.LogInformation($"ProductService: DeactivateProductAsync called for ID {id}");
                return await _productDAO.DeactivateProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in DeactivateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Deactivate product for specific user
        /// </summary>
        public async Task<bool> DeactivateProductAsync(string userId, int id)
        {
            try
            {
                _logger.LogInformation($"ProductService: DeactivateProductAsync called for userId {userId} and ID {id}");
                return await _productDAO.DeactivateProductAsync(userId, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in DeactivateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Activate product
        /// </summary>
        public async Task<bool> ActivateProductAsync(int id)
        {
            try
            {
                _logger.LogInformation($"ProductService: ActivateProductAsync called for ID {id}");
                return await _productDAO.ActivateProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in ActivateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Activate product for specific user
        /// </summary>
        public async Task<bool> ActivateProductAsync(string userId, int id)
        {
            try
            {
                _logger.LogInformation($"ProductService: ActivateProductAsync called for userId {userId} and ID {id}");
                return await _productDAO.ActivateProductAsync(userId, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in ActivateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get product statistics
        /// </summary>
        public async Task<ProductStatisticsVM> GetProductStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("ProductService: GetProductStatisticsAsync called");
                return await _productDAO.GetProductStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService: Error in GetProductStatisticsAsync");
                throw;
            }
        }

        /// <summary>
        /// Update product (overload for Products entity)
        /// </summary>
        public async Task<bool> UpdateProductAsync(Products product)
        {
            try
            {
                _logger.LogInformation($"ProductService: UpdateProductAsync called for product {product.Id}");

                if (product == null)
                {
                    throw new ArgumentException("Product data is required.");
                }

                var updateModel = new UpdateProductVM
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    ImagePath = product.ImagePath
                };

                return await _productDAO.UpdateProductAsync(product.Id, updateModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in UpdateProductAsync for product {product?.Id}");
                throw;
            }
        }

        /// <summary>
        /// Get products by category with count
        /// </summary>
        public async Task<List<ProductVM>> GetProductsByCategoryWithCountAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation($"ProductService: GetProductsByCategoryWithCountAsync called with category {categoryId}");
                return await _productDAO.GetProductsByCategoryWithCountAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProductService: Error in GetProductsByCategoryWithCountAsync for category {categoryId}");
                throw;
            }
        }
    }
}

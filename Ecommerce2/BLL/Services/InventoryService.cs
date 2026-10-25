using DAL.DTO;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_application.SERVICES
{
    public interface IInventoryService
    {
        Task<List<InventoryVM>> GetAllInventoryAsync();
        Task<InventoryVM> GetInventoryByProductIdAsync(int productId);
        Task<List<InventoryVM>> GetLowStockInventoryAsync(int threshold);
        Task<List<InventoryVM>> GetOutOfStockInventoryAsync();
        Task<bool> UpdateInventoryAsync(int productId, UpdateInventoryVM model);
        Task<bool> AdjustInventoryAsync(int productId, int quantityChange);
        Task<InventorySummaryVM> GetInventorySummaryAsync();
    }

    public class InventoryService : IInventoryService
    {
        private readonly InventoryDAO _inventoryDAO;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(InventoryDAO inventoryDAO, ILogger<InventoryService> logger)
        {
            _inventoryDAO = inventoryDAO;
            _logger = logger;
        }

        /// <summary>
        /// Get all inventory records (products with quantity)
        /// </summary>
        public async Task<List<InventoryVM>> GetAllInventoryAsync()
        {
            try
            {
                _logger.LogInformation("InventoryService: GetAllInventoryAsync called");
                var inventories = await _inventoryDAO.GetAllInventoryAsync();
                _logger.LogInformation($"InventoryService: Retrieved {inventories.Count} inventory records");

                return inventories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryService: Error in GetAllInventoryAsync");
                throw;
            }
        }

        /// <summary>
        /// Get inventory by product ID
        /// </summary>
        public async Task<InventoryVM> GetInventoryByProductIdAsync(int productId)
        {
            try
            {
                _logger.LogInformation($"InventoryService: GetInventoryByProductIdAsync called with product ID {productId}");

                // Validation
                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID issue.");
                }

                var inventory = await _inventoryDAO.GetInventoryByProductIdAsync(productId);

                if (inventory == null)
                {
                    _logger.LogWarning($"InventoryService: Inventory for product ID {productId} not found");
                    throw new KeyNotFoundException($"Inventory for product ID {productId} not found.");
                }

                return inventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"InventoryService: Error in GetInventoryByProductIdAsync for product {productId}");
                throw;
            }
        }

        /// <summary>
        /// Get low stock inventory items
        /// </summary>
        public async Task<List<InventoryVM>> GetLowStockInventoryAsync(int threshold)
        {
            try
            {
                _logger.LogInformation($"InventoryService: GetLowStockInventoryAsync called with threshold {threshold}");

                // Validation
                if (threshold < 0)
                {
                    throw new ArgumentException("Threshold cannot be negative.");
                }

                var inventories = await _inventoryDAO.GetLowStockInventoryAsync(threshold);
                _logger.LogInformation($"InventoryService: Found {inventories.Count} low stock items with threshold {threshold}");

                return inventories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"InventoryService: Error in GetLowStockInventoryAsync with threshold {threshold}");
                throw;
            }
        }

        /// <summary>
        /// Get out of stock inventory items
        /// </summary>
        public async Task<List<InventoryVM>> GetOutOfStockInventoryAsync()
        {
            try
            {
                _logger.LogInformation("InventoryService: GetOutOfStockInventoryAsync called");
                var inventories = await _inventoryDAO.GetOutOfStockInventoryAsync();
                _logger.LogInformation($"InventoryService: Found {inventories.Count} out of stock items");

                return inventories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryService: Error in GetOutOfStockInventoryAsync");
                throw;
            }
        }

        /// <summary>
        /// Update product inventory quantity by product ID
        /// </summary>
        public async Task<bool> UpdateInventoryAsync(int productId, UpdateInventoryVM model)
        {
            try
            {
                _logger.LogInformation($"InventoryService: UpdateInventoryAsync called for product ID {productId}");

                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID issue.");
                }

                if (model == null)
                {
                    throw new ArgumentException("Inventory data is required.");
                }

                if (model.Quantity < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative.");
                }

                var result = await _inventoryDAO.UpdateInventoryAsync(productId, model);

                if (!result)
                {
                    _logger.LogWarning($"InventoryService: Product with ID {productId} not found for update");
                    throw new KeyNotFoundException($"Product with ID {productId} not found.");
                }

                _logger.LogInformation($"InventoryService: Product {productId} inventory updated successfully to quantity {model.Quantity}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"InventoryService: Error in UpdateInventoryAsync for product {productId}");
                throw;
            }
        }

        /// <summary>
        /// Adjust product inventory quantity (add or subtract)
        /// </summary>
        public async Task<bool> AdjustInventoryAsync(int productId, int quantityChange)
        {
            try
            {
                _logger.LogInformation($"InventoryService: AdjustInventoryAsync called for product ID {productId} with change {quantityChange}");

                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID issue.");
                }

                var inventory = await _inventoryDAO.GetInventoryByProductIdAsync(productId);

                if (inventory == null)
                {
                    _logger.LogWarning($"InventoryService: Product inventory with ID {productId} not found for adjustment");
                    throw new KeyNotFoundException($"Product with ID {productId} not found.");
                }

                int newQuantity = inventory.Quantity + quantityChange;

                if (newQuantity < 0)
                {
                    throw new ArgumentException($"Adjustment would result in negative quantity. Current: {inventory.Quantity}, Change: {quantityChange}");
                }

                var result = await _inventoryDAO.AdjustInventoryAsync(productId, quantityChange);

                if (!result)
                {
                    _logger.LogWarning($"InventoryService: Failed to adjust inventory for product ID {productId}");
                    return false;
                }

                _logger.LogInformation($"InventoryService: Product {productId} inventory adjusted by {quantityChange}. New quantity: {newQuantity}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"InventoryService: Error in AdjustInventoryAsync for product {productId}");
                throw;
            }
        }

        /// <summary>
        /// Get inventory summary statistics
        /// </summary>
        public async Task<InventorySummaryVM> GetInventorySummaryAsync()
        {
            try
            {
                _logger.LogInformation("InventoryService: GetInventorySummaryAsync called");
                var summary = await _inventoryDAO.GetInventorySummaryAsync();
                _logger.LogInformation($"InventoryService: Inventory summary retrieved - Total: {summary.TotalItems}, Low Stock: {summary.LowStockItems}, Out of Stock: {summary.OutOfStockItems}");

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryService: Error in GetInventorySummaryAsync");
                throw;
            }
        }
    }
}

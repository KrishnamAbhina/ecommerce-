using DAL.DTO;
using ecommerce_application.DATA;
using ecommerce_application.MODELS.DB_MODELS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class InventoryDAO
    {
        private readonly EcommerceDbContext _context;

        public InventoryDAO(EcommerceDbContext context)
        {
            _context = context;
        }

        // ============ READ ============

        /// <summary>
        /// Get all inventory records (products with quantity)
        /// </summary>
        public async Task<List<InventoryVM>> GetAllInventoryAsync()
        {
            var inventories = await _context.Products
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Id)
                .Select(p => new InventoryVM
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = p.Quantity,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return inventories;
        }

        /// <summary>
        /// Get inventory by product ID
        /// </summary>
        public async Task<InventoryVM> GetInventoryByProductIdAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return null;
            }

            return new InventoryVM
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = product.Quantity,
                UpdatedAt = product.UpdatedAt
            };
        }

        /// <summary>
        /// Get low stock inventory items
        /// </summary>
        public async Task<List<InventoryVM>> GetLowStockInventoryAsync(int threshold)
        {
            var inventories = await _context.Products
                .Where(p => p.Quantity <= threshold && p.IsActive)
                .OrderBy(p => p.Quantity)
                .Select(p => new InventoryVM
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = p.Quantity,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return inventories;
        }

        /// <summary>
        /// Get out of stock inventory items
        /// </summary>
        public async Task<List<InventoryVM>> GetOutOfStockInventoryAsync()
        {
            var inventories = await _context.Products
                .Where(p => p.Quantity == 0 && p.IsActive)
                .OrderByDescending(p => p.Id)
                .Select(p => new InventoryVM
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = p.Quantity,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return inventories;
        }

        // ============ UPDATE ============

        /// <summary>
        /// Update product inventory quantity by product ID
        /// </summary>
        public async Task<bool> UpdateInventoryAsync(int productId, UpdateInventoryVM model)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return false;
            }

            product.Quantity = model.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Adjust product inventory quantity (add or subtract)
        /// </summary>
        public async Task<bool> AdjustInventoryAsync(int productId, int quantityChange)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return false;
            }

            product.Quantity += quantityChange;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ STATISTICS ============

        /// <summary>
        /// Get inventory summary statistics
        /// </summary>
        public async Task<InventorySummaryVM> GetInventorySummaryAsync()
        {
            var totalItems = await _context.Products.CountAsync(p => p.IsActive);
            var lowStockItems = await _context.Products.CountAsync(p => p.Quantity <= 10 && p.IsActive);
            var outOfStockItems = await _context.Products.CountAsync(p => p.Quantity == 0 && p.IsActive);
            var totalInventoryValue = await _context.Products
                .Where(p => p.IsActive)
                .SumAsync(p => p.Price * p.Quantity);

            return new InventorySummaryVM
            {
                TotalItems = totalItems,
                LowStockItems = lowStockItems,
                OutOfStockItems = outOfStockItems,
                TotalInventoryValue = totalInventoryValue
            };
        }
    }
}

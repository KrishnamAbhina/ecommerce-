using DAL.DTO;
using DAL.Services;
using ecommerce_application.MODELS.DB_MODELS;
using ecommerce_application.DATA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_application.SERVICES
{
    public interface ICartService
    {
        Task<int> AddToCartAsync(string userId, int productId, int quantity);
        Task<List<CartItemVM>> GetAllCartItemsAsync(string userId);
        Task<CartItemVM> GetCartItemByIdAsync(int cartItemId, string userId);
        Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity, string userId);
        Task<bool> RemoveFromCartAsync(int cartItemId, string userId);
        Task<bool> ClearCartAsync(string userId);
        Task<CartSummaryVM> GetCartSummaryAsync(string userId);
    }

    public class CartService : ICartService
    {
        private readonly CartDAO _cartDAO;
        private readonly ProductDAO _productDAO;
        private readonly EcommerceDbContext _context;
        private readonly ILogger<CartService> _logger;
        private readonly IInventoryService _inventoryService;

        public CartService(CartDAO cartDAO, ProductDAO productDAO, EcommerceDbContext context, ILogger<CartService> logger, IInventoryService inventoryService)
        {
            _cartDAO = cartDAO;
            _productDAO = productDAO;
            _context = context;
            _logger = logger;
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Add product to cart for specific user
        /// </summary>
        public async Task<int> AddToCartAsync(string userId, int productId, int quantity)
        {
            try
            {
                _logger.LogInformation($"CartService: AddToCartAsync called with userId {userId}, productId {productId}");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID Invalid.");
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                // Validate that user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    throw new KeyNotFoundException($"User with ID {userId} not found.");
                }

                // Fetch product to get price and validate it exists
                var product = await _productDAO.GetProductByIdAsync(productId);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {productId} not found.");
                }

                if (!product.IsActive)
                {
                    throw new InvalidOperationException($"Product with ID {productId} is not available.");
                }

                // Check inventory availability
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
                if (inventory == null)
                {
                    throw new KeyNotFoundException($"Inventory not found for product ID {productId}.");
                }

                if (inventory.Quantity < quantity)
                {
                    throw new InvalidOperationException($"Insufficient inventory. Available: {inventory.Quantity}, Requested: {quantity}");
                }

                // Check if product already in user's cart
                var existingItem = await _cartDAO.GetCartItemByProductIdAsync(productId, userId);

                if (existingItem != null)
                {
                    // Update quantity for existing item
                    existingItem.Quantity += quantity;
                    await _cartDAO.UpdateCartItemAsync(existingItem.Id, new UpdateCartItemVM { Quantity = existingItem.Quantity });

                    _logger.LogInformation($"CartService: Cart item quantity increased by {quantity}");
                }
                else
                {
                    // Create new cart item with price fetched from product
                    var cartItem = new CartItemsModel
                    {
                        UserId = userId,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price,
                        AddedAt = DateTime.UtcNow
                    };

                    await _cartDAO.AddToCartAsync(cartItem);

                    _logger.LogInformation($"CartService: New cart item added - productId: {productId}, quantity: {quantity}, price: {product.Price}");
                }

                // Decrease inventory by the quantity added to cart
                await _inventoryService.AdjustInventoryAsync(productId, -quantity);
                _logger.LogInformation($"CartService: Inventory decreased by {quantity} for product {productId}");

                _logger.LogInformation($"CartService: Product {productId} added to cart successfully");
                return productId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CartService: Error in AddToCartAsync");
                throw;
            }
        }

        /// <summary>
        /// Get all cart items for specific user
        /// </summary>
        public async Task<List<CartItemVM>> GetAllCartItemsAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"CartService: GetAllCartItemsAsync called for userId {userId}");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var cartItems = await _cartDAO.GetAllCartItemsAsync(userId);

                if (cartItems == null || !cartItems.Any())
                {
                    return new List<CartItemVM>();
                }

                _logger.LogInformation($"CartService: Retrieved {cartItems.Count} cart items for user {userId}");
                return cartItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CartService: Error in GetAllCartItemsAsync");
                throw;
            }
        }

        /// <summary>
        /// Get cart item by ID for specific user
        /// </summary>
        public async Task<CartItemVM> GetCartItemByIdAsync(int cartItemId, string userId)
        {
            try
            {
                _logger.LogInformation($"CartService: GetCartItemByIdAsync called with cartItemId {cartItemId}, userId {userId}");

                if (cartItemId <= 0)
                {
                    throw new ArgumentException("Cart Item ID must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var cartItem = await _cartDAO.GetCartItemByIdAsync(cartItemId, userId);

                if (cartItem == null)
                {
                    throw new KeyNotFoundException($"Cart Item with ID {cartItemId} not found.");
                }

                _logger.LogInformation($"CartService: Cart item retrieved successfully with ID {cartItemId}");
                return cartItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CartService: Error in GetCartItemByIdAsync");
                throw;
            }
        }

        /// <summary>
        /// Update cart item quantity for specific user
        /// </summary>
        public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity, string userId)
        {
            try
            {
                _logger.LogInformation($"CartService: UpdateCartItemQuantityAsync called with cartItemId {cartItemId}, userId {userId}");

                if (cartItemId <= 0)
                {
                    throw new ArgumentException("Cart Item ID must be greater than 0.");
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                // Get current cart item to calculate quantity difference
                var currentCartItem = await _cartDAO.GetCartItemByIdAsync(cartItemId, userId);
                if (currentCartItem == null)
                {
                    throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
                }

                // Check inventory for the difference
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(currentCartItem.ProductId);
                if (inventory == null)
                {
                    throw new KeyNotFoundException($"Inventory not found for product ID {currentCartItem.ProductId}.");
                }

                int quantityDifference = quantity - currentCartItem.Quantity;

                // If increasing quantity, check if enough inventory
                if (quantityDifference > 0 && inventory.Quantity < quantityDifference)
                {
                    throw new InvalidOperationException($"Insufficient inventory. Available: {inventory.Quantity}, Requested: {quantityDifference}");
                }

                var model = new UpdateCartItemVM { Quantity = quantity };
                var result = await _cartDAO.UpdateCartItemAsync(cartItemId, model);

                if (result)
                {
                    // Adjust inventory based on quantity difference
                    if (quantityDifference != 0)
                    {
                        await _inventoryService.AdjustInventoryAsync(currentCartItem.ProductId, -quantityDifference);
                        _logger.LogInformation($"CartService: Inventory adjusted by {-quantityDifference} for product {currentCartItem.ProductId}");
                    }

                    _logger.LogInformation($"CartService: Cart item quantity updated successfully");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CartService: Error in UpdateCartItemQuantityAsync");
                throw;
            }
        }

        /// <summary>
        /// Remove item from cart for specific user
        /// </summary>
        public async Task<bool> RemoveFromCartAsync(int cartItemId, string userId)
        {
            try
            {
                _logger.LogInformation($"CartService: RemoveFromCartAsync called with cartItemId {cartItemId}, userId {userId}");

                if (cartItemId <= 0)
                {
                    throw new ArgumentException("Cart Item ID must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                // Get cart item details before deletion to restore inventory
                var cartItem = await _cartDAO.GetCartItemByIdAsync(cartItemId, userId);
                if (cartItem == null)
                {
                    throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
                }

                var result = await _cartDAO.DeleteCartItemAsync(cartItemId);

                if (result)
                {
                    // Restore inventory when item is removed from cart
                    var inventory = await _inventoryService.GetInventoryByProductIdAsync(cartItem.ProductId);
                    if (inventory != null)
                    {
                        await _inventoryService.AdjustInventoryAsync(cartItem.ProductId, cartItem.Quantity);
                        _logger.LogInformation($"CartService: Inventory restored by {cartItem.Quantity} for product {cartItem.ProductId}");
                    }

                    _logger.LogInformation($"CartService: Cart item removed successfully");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CartService: Error in RemoveFromCartAsync");
                throw;
            }
        }

        /// <summary>
        /// Clear all items from cart for specific user
        /// </summary>
        public async Task<bool> ClearCartAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"CartService: ClearCartAsync called for userId {userId}");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                // Get all cart items before deletion to restore inventory
                var cartItems = await _cartDAO.GetAllCartItemsAsync(userId);

                var result = await _cartDAO.DeleteAllCartItemsAsync(userId);

                if (result && cartItems != null && cartItems.Any())
                {
                    // Restore inventory for all items
                    foreach (var item in cartItems)
                    {
                        var inventory = await _inventoryService.GetInventoryByProductIdAsync(item.ProductId);
                        if (inventory != null)
                        {
                            await _inventoryService.AdjustInventoryAsync(item.ProductId, item.Quantity);
                            _logger.LogInformation($"CartService: Inventory restored by {item.Quantity} for product {item.ProductId}");
                        }
                    }

                    _logger.LogInformation("CartService: Cart cleared successfully and inventory restored");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CartService: Error in ClearCartAsync");
                throw;
            }
        }

        /// <summary>
        /// Get cart summary for specific user
        /// </summary>
        public async Task<CartSummaryVM> GetCartSummaryAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"CartService: GetCartSummaryAsync called for userId {userId}");

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var summary = await _cartDAO.GetCartSummaryAsync(userId);

                _logger.LogInformation($"CartService: Cart summary - Items: {summary.TotalItems}, Price: {summary.TotalPrice}");
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CartService: Error in GetCartSummaryAsync");
                throw;
            }
        }
    }
}

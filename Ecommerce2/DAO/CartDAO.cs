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
    public class CartDAO
    {
        private readonly EcommerceDbContext _context;

        public CartDAO(EcommerceDbContext context)
        {
            _context = context;
        }

        // ============ CREATE ============

        /// <summary>
        /// Add item to cart items
        /// </summary>
        public async Task<int> AddToCartAsync(CartItemsModel cartItem)
        {
            try
            {
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                return cartItem.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding cart item: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        // ============ READ ============

        /// <summary>
        /// Get cart item by ID for specific user
        /// </summary>
        public async Task<CartItemVM> GetCartItemByIdAsync(int id, string userId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.UserId == userId);

            if (cartItem == null)
            {
                return null;
            }

            return new CartItemVM
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                TotalPrice = cartItem.Price * cartItem.Quantity,
                AddedAt = cartItem.AddedAt
            };
        }

        /// <summary>
        /// Get cart item by product ID for specific user
        /// </summary>
        public async Task<CartItemsModel> GetCartItemByProductIdAsync(int productId, string userId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);
        }

        /// <summary>
        /// Get all cart items for specific user
        /// </summary>
        public async Task<List<CartItemVM>> GetAllCartItemsAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .OrderByDescending(ci => ci.Id)
                .Select(ci => new CartItemVM
                {
                    Id = ci.Id,
                    UserId = ci.UserId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Price,
                    TotalPrice = ci.Price * ci.Quantity,
                    AddedAt = ci.AddedAt
                })
                .ToListAsync();

            return cartItems;
        }

        // ============ UPDATE ============

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        public async Task<bool> UpdateCartItemAsync(int id, UpdateCartItemVM model)
        {
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return false;
            }

            cartItem.Quantity = model.Quantity;

            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ DELETE ============

        /// <summary>
        /// Delete cart item by ID
        /// </summary>
        public async Task<bool> DeleteCartItemAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Delete all cart items for specific user
        /// </summary>
        public async Task<bool> DeleteAllCartItemsAsync(string userId)
        {
            var allCartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (allCartItems == null || !allCartItems.Any())
            {
                return true;
            }

            _context.CartItems.RemoveRange(allCartItems);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ ADDITIONAL METHODS ============

        /// <summary>
        /// Get cart items for a specific user
        /// </summary>
        public async Task<CartVM> GetCartByUserIdAsync(string userId)
        {
            var allCartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (allCartItems == null || !allCartItems.Any())
            {
                return null;
            }

            var cartItems = allCartItems.Select(ci => new CartItemVM
            {
                Id = ci.Id,
                UserId = ci.UserId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Quantity = ci.Quantity,
                Price = ci.Price,
                TotalPrice = ci.Price * ci.Quantity,
                AddedAt = ci.AddedAt
            }).ToList();

            var totalPrice = cartItems.Sum(ci => ci.TotalPrice);
            var totalItems = cartItems.Sum(ci => ci.Quantity);

            return new CartVM
            {
                UserId = userId,
                CartItems = cartItems,
                TotalPrice = totalPrice,
                TotalItems = totalItems,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        /// <summary>
        /// Get cart summary for specific user
        /// </summary>
        public async Task<CartSummaryVM> GetCartSummaryAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return new CartSummaryVM
                {
                    UserId = userId,
                    TotalItems = 0,
                    TotalPrice = 0,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            var totalItems = cartItems.Sum(ci => ci.Quantity);
            var totalPrice = cartItems.Sum(ci => ci.Price * ci.Quantity);

            return new CartSummaryVM
            {
                UserId = userId,
                TotalItems = totalItems,
                TotalPrice = totalPrice,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Check if product exists in user's cart
        /// </summary>
        public async Task<bool> DoesProductExistInCartAsync(int productId, string userId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.ProductId == productId && ci.UserId == userId);
        }

        /// <summary>
        /// Get cart items count for specific user
        /// </summary>
        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .CountAsync();
        }
    }
}

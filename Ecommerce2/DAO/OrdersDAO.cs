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
    public class OrdersDAO
    {
        private readonly EcommerceDbContext _context;

        public OrdersDAO(EcommerceDbContext context)
        {
            _context = context;
        }

        // ============ CREATE ============

        /// <summary>
        /// Create a new order with calculated total amount from cart
        /// </summary>
        public async Task<int> CreateOrderAsync(CreateOrderVM model, decimal totalAmount)
        {
            var order = new Orders
            {
                UserId = model.UserId,
                AddressId = model.AddressId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "placed",
                StatusId = 1
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items from cart and decrease inventory
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == model.UserId)
                .ToListAsync();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItemsModel
                {
                    UserId = model.UserId,
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
                _context.OrderItems.Add(orderItem);

                // Decrease inventory
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
                if (product != null)
                {
                    product.Quantity -= cartItem.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    _context.Products.Update(product);
                }
            }

            await _context.SaveChangesAsync();
            return order.Id;
        }

        // ============ READ ============

        /// <summary>
        /// Get all orders
        /// </summary>
        public async Task<List<OrderVM>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderVM
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    AddressId = o.AddressId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                })
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        public async Task<OrderVM> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            return new OrderVM
            {
                Id = order.Id,
                UserId = order.UserId,
                AddressId = order.AddressId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };
        }

        /// <summary>
        /// Get all orders by user ID
        /// </summary>
        public async Task<List<OrderVM>> GetOrdersByUserAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderVM
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    AddressId = o.AddressId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                })
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Get order with details (including order items)
        /// </summary>
        public async Task<OrderDetailsVM> GetOrderWithDetailsAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            return new OrderDetailsVM
            {
                Id = order.Id,
                UserId = order.UserId,
                AddressId = order.AddressId,
                AddressStreet = order.Address.Street,
                AddressCity = order.Address.City,
                AddressState = order.Address.State,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDetailsVM
                {
                    OrderItemId = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    TotalPrice = oi.Quantity * oi.Price
                }).ToList()
            };
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        public async Task<List<OrderVM>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _context.Orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderVM
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    AddressId = o.AddressId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                })
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Get orders by date range
        /// </summary>
        public async Task<List<OrderVM>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderVM
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    AddressId = o.AddressId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                })
                .ToListAsync();

            return orders;
        }

        // ============ UPDATE ============

        /// <summary>
        /// Update order status
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderVM model)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return false;
            }

            order.Status = model.Status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Update order
        /// </summary>
        public async Task<bool> UpdateOrderAsync(int id, Orders orderModel)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return false;
            }

            order.Status = orderModel.Status;
            order.TotalAmount = orderModel.TotalAmount;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ DELETE ============

        /// <summary>
        /// Delete an order
        /// </summary>
        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============ STATISTICS ============

        /// <summary>
        /// Get order summary statistics
        /// </summary>
        public async Task<OrderSummaryVM> GetOrderSummaryAsync()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
            var cancelledOrders = await _context.Orders.CountAsync(o => o.Status == "Cancelled");
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            return new OrderSummaryVM
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue
            };
        }

        /// <summary>
        /// Get user's total order count
        /// </summary>
        public async Task<int> GetUserOrderCountAsync(string userId)
        {
            var count = await _context.Orders
                .CountAsync(o => o.UserId == userId);

            return count;
        }

        /// <summary>
        /// Get user's total spending
        /// </summary>
        public async Task<decimal> GetUserTotalSpendingAsync(string userId)
        {
            var total = await _context.Orders
                .Where(o => o.UserId == userId)
                .SumAsync(o => o.TotalAmount);

            return total;
        }

        /// <summary>
        /// Check if order belongs to user
        /// </summary>
        public async Task<bool> DoesOrderBelongToUserAsync(int orderId, string userId)
        {
            var exists = await _context.Orders
                .AnyAsync(o => o.Id == orderId && o.UserId == userId);

            return exists;
        }
    }
}

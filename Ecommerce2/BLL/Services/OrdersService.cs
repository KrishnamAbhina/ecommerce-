using DAL.DTO;
using DAL.Services;
using ecommerce_application.MODELS.DB_MODELS;

namespace ecommerce_application.SERVICES
{
    public interface IOrdersService
    {
        Task<int> CreateOrderAsync(CreateOrderVM model);
        Task<List<OrderVM>> GetAllOrdersAsync();
        Task<OrderVM> GetOrderByIdAsync(int id);
        Task<List<OrderVM>> GetOrdersByUserAsync(string userId);
        Task<OrderDetailsVM> GetOrderWithDetailsAsync(int id);
        Task<List<OrderVM>> GetOrdersByStatusAsync(string status);
        Task<List<OrderVM>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderVM model);
        Task<bool> UpdateOrderAsync(int id, Orders orderModel);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderSummaryVM> GetOrderSummaryAsync();
        Task<int> GetUserOrderCountAsync(string userId);
        Task<decimal> GetUserTotalSpendingAsync(string userId);
    }

    public class OrdersService : IOrdersService
    {
        private readonly OrdersDAO _ordersDAO;
        private readonly CartDAO _cartDAO;
        private readonly ILogger<OrdersService> _logger;

        public OrdersService(OrdersDAO ordersDAO, CartDAO cartDAO, ILogger<OrdersService> logger)
        {
            _ordersDAO = ordersDAO;
            _cartDAO = cartDAO;
            _logger = logger;
        }

        /// <summary>
        /// Create a new order from cart items (Status defaults to "placed", TotalAmount calculated from cart)
        /// </summary>
        public async Task<int> CreateOrderAsync(CreateOrderVM model)
        {
            try
            {
                _logger.LogInformation($"OrdersService: CreateOrderAsync called for user {model.UserId}");

                // Validation
                if (model == null)
                {
                    throw new ArgumentException("Order data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.UserId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                if (model.AddressId <= 0)
                {
                    throw new ArgumentException("Valid address ID is required.");
                }

                if (string.IsNullOrWhiteSpace(model.PaymentMode))
                {
                    throw new ArgumentException("Payment mode is required.");
                }

                var validPaymentModes = new[] { "COD", "online" };
                if (!validPaymentModes.Contains(model.PaymentMode.ToUpper()))
                {
                    throw new ArgumentException("Invalid payment mode. Valid modes are: COD, online");
                }

                if (!model.PlaceOrder)
                {
                    throw new ArgumentException("Please confirm order placement (placeOrder must be true).");
                }

                // user cart and calculations etcc
                var userCart = await _cartDAO.GetCartByUserIdAsync(model.UserId);
                if (userCart == null || userCart.CartItems == null || userCart.CartItems.Count == 0)
                {
                    throw new ArgumentException("Cart is empty. Cannot create order without items.");
                }

                decimal totalAmount = userCart.TotalPrice;
                _logger.LogInformation($"OrdersService: Total amount calculated cart: {totalAmount} for user {model.UserId}");

                var orderId = await _ordersDAO.CreateOrderAsync(model, totalAmount);
                _logger.LogInformation($"OrdersService: Order created successfully with ID {orderId} for user {model.UserId}");

                // clearing th e cart
                await _cartDAO.DeleteAllCartItemsAsync(model.UserId);
                _logger.LogInformation($"OrdersService: Cart cleared after order placement");

                return orderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrdersService: Error in CreateOrderAsync");
                throw;
            }
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        public async Task<List<OrderVM>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("OrdersService: GetAllOrdersAsync called");
                var orders = await _ordersDAO.GetAllOrdersAsync();
                _logger.LogInformation($"OrdersService: Retrieved {orders.Count} orders");

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrdersService: Error in GetAllOrdersAsync");
                throw;
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        public async Task<OrderVM> GetOrderByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetOrderByIdAsync called with ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than 0.");
                }

                var order = await _ordersDAO.GetOrderByIdAsync(id);

                if (order == null)
                {
                    _logger.LogWarning($"OrdersService: Order with ID {id} not found");
                    throw new KeyNotFoundException($"Order with ID {id} not found.");
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetOrderByIdAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get orders by user
        /// </summary>
        public async Task<List<OrderVM>> GetOrdersByUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetOrdersByUserAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var orders = await _ordersDAO.GetOrdersByUserAsync(userId);
                _logger.LogInformation($"OrdersService: Retrieved {orders.Count} orders for user {userId}");

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetOrdersByUserAsync for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Get order with details
        /// </summary>
        public async Task<OrderDetailsVM> GetOrderWithDetailsAsync(int id)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetOrderWithDetailsAsync called with ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than 0.");
                }

                var order = await _ordersDAO.GetOrderWithDetailsAsync(id);

                if (order == null)
                {
                    _logger.LogWarning($"OrdersService: Order with ID {id} not found");
                    throw new KeyNotFoundException($"Order with ID {id} not found.");
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetOrderWithDetailsAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        public async Task<List<OrderVM>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetOrdersByStatusAsync called with status {status}");

                // Validation
                if (string.IsNullOrWhiteSpace(status))
                {
                    throw new ArgumentException("Status is required.");
                }

                // Business Logic: Validate status
                var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
                if (!validStatuses.Contains(status))
                {
                    throw new ArgumentException($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
                }

                var orders = await _ordersDAO.GetOrdersByStatusAsync(status);
                _logger.LogInformation($"OrdersService: Retrieved {orders.Count} orders with status {status}");

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetOrdersByStatusAsync for status {status}");
                throw;
            }
        }

        /// <summary>
        /// Get orders by date range
        /// </summary>
        public async Task<List<OrderVM>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetOrdersByDateRangeAsync called for range {startDate} to {endDate}");

                // Business Logic: Validate date range
                if (endDate < startDate)
                {
                    throw new ArgumentException("End date must be greater than or equal to start date.");
                }

                var orders = await _ordersDAO.GetOrdersByDateRangeAsync(startDate, endDate);
                _logger.LogInformation($"OrdersService: Retrieved {orders.Count} orders in date range");

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetOrdersByDateRangeAsync");
                throw;
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderVM model)
        {
            try
            {
                _logger.LogInformation($"OrdersService: UpdateOrderStatusAsync called for ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than 0.");
                }

                if (model == null)
                {
                    throw new ArgumentException("Order data is required.");
                }

                if (string.IsNullOrWhiteSpace(model.Status))
                {
                    throw new ArgumentException("Order status is required.");
                }

                // Business Logic: Validate status
                var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
                if (!validStatuses.Contains(model.Status))
                {
                    throw new ArgumentException($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
                }

                var result = await _ordersDAO.UpdateOrderStatusAsync(id, model);

                if (!result)
                {
                    _logger.LogWarning($"OrdersService: Order with ID {id} not found for update");
                    throw new KeyNotFoundException($"Order with ID {id} not found.");
                }

                _logger.LogInformation($"OrdersService: Order with ID {id} status updated to {model.Status}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in UpdateOrderStatusAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Update order
        /// </summary>
        public async Task<bool> UpdateOrderAsync(int id, Orders orderModel)
        {
            try
            {
                _logger.LogInformation($"OrdersService: UpdateOrderAsync called for ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than 0.");
                }

                if (orderModel == null)
                {
                    throw new ArgumentException("Order data is required.");
                }

                if (orderModel.TotalAmount <= 0)
                {
                    throw new ArgumentException("Order total amount must be greater than 0.");
                }

                // Business Logic: Validate status
                var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
                if (!validStatuses.Contains(orderModel.Status))
                {
                    throw new ArgumentException($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
                }

                var result = await _ordersDAO.UpdateOrderAsync(id, orderModel);

                if (!result)
                {
                    _logger.LogWarning($"OrdersService: Order with ID {id} not found for update");
                    throw new KeyNotFoundException($"Order with ID {id} not found.");
                }

                _logger.LogInformation($"OrdersService: Order with ID {id} updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in UpdateOrderAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Delete order
        /// </summary>
        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                _logger.LogInformation($"OrdersService: DeleteOrderAsync called for ID {id}");

                // Validation
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than 0.");
                }

                var order = await _ordersDAO.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning($"OrdersService: Order with ID {id} not found for deletion");
                    throw new KeyNotFoundException($"Order with ID {id} not found.");
                }

                // Business Logic: Cannot delete delivered or processing orders
                if (order.Status == "Delivered" || order.Status == "Processing")
                {
                    throw new InvalidOperationException($"Cannot delete order with status '{order.Status}'. Only orders with status Pending or Cancelled can be deleted.");
                }

                var result = await _ordersDAO.DeleteOrderAsync(id);

                if (!result)
                {
                    _logger.LogWarning($"OrdersService: Failed to delete order {id}");
                    return false;
                }

                _logger.LogInformation($"OrdersService: Order with ID {id} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in DeleteOrderAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get order summary statistics
        /// </summary>
        public async Task<OrderSummaryVM> GetOrderSummaryAsync()
        {
            try
            {
                _logger.LogInformation("OrdersService: GetOrderSummaryAsync called");
                var summary = await _ordersDAO.GetOrderSummaryAsync();
                _logger.LogInformation($"OrdersService: Order summary retrieved - Total: {summary.TotalOrders}, Revenue: {summary.TotalRevenue}");

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrdersService: Error in GetOrderSummaryAsync");
                throw;
            }
        }

        /// <summary>
        /// Get user order count
        /// </summary>
        public async Task<int> GetUserOrderCountAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetUserOrderCountAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var count = await _ordersDAO.GetUserOrderCountAsync(userId);
                _logger.LogInformation($"OrdersService: User {userId} has {count} orders");

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetUserOrderCountAsync for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Get user total spending
        /// </summary>
        public async Task<decimal> GetUserTotalSpendingAsync(string userId)
        {
            try
            {
                _logger.LogInformation($"OrdersService: GetUserTotalSpendingAsync called for user {userId}");

                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("User ID is required.");
                }

                var total = await _ordersDAO.GetUserTotalSpendingAsync(userId);
                _logger.LogInformation($"OrdersService: User {userId} total spending: {total}");

                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OrdersService: Error in GetUserTotalSpendingAsync for user {userId}");
                throw;
            }
        }
    }
}

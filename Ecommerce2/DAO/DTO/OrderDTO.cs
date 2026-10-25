namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreateOrderVM
    {
        public required string UserId { get; set; }
        public int AddressId { get; set; }
        public required string PaymentMode { get; set; }  // "COD" or "online"
        public bool PlaceOrder { get; set; }     // Confirmation: true = yes, false = no
    }

    // ===== UPDATE DTO =====
    public class UpdateOrderVM
    {
        public required string Status { get; set; }
    }

    // ===== RESPONSE DTO =====
    public class OrderVM
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int AddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }
    }

    // ===== ORDER WITH DETAILS DTO =====
    public class OrderDetailsVM
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int AddressId { get; set; }
        public required string AddressStreet { get; set; }
        public required string AddressCity { get; set; }
        public required string AddressState { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }
        public List<OrderItemDetailsVM> OrderItems { get; set; } = new List<OrderItemDetailsVM>();
    }

    // ===== ORDER ITEM DETAILS DTO =====
    public class OrderItemDetailsVM
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // ===== ORDER SUMMARY DTO =====
    public class OrderSummaryVM
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}

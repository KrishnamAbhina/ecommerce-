
namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreateInventoryVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    // ===== UPDATE DTO =====
    public class UpdateInventoryVM
    {
        public int Quantity { get; set; }
    }

    // ===== RESPONSE DTO =====
    public class InventoryVM
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ===== INVENTORY SUMMARY DTO =====
    public class InventorySummaryVM
    {
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }
}

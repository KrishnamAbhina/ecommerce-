namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreateProductVM
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }

    // ===== UPDATE DTO =====
    public class UpdateProductVM
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }

    // ===== RESPONSE DTO =====
    public class ProductVM
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public int Quantity { get; set; }
    }

    // ===== PRODUCT STATISTICS DTO =====
    public class ProductStatisticsVM
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }
}
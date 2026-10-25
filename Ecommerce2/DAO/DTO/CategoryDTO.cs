namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreateCategoryVM
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    // ===== UPDATE DTO =====
    public class UpdateCategoryVM
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    // ===== RESPONSE DTO =====
    public class CategoryVM
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
    }

    // ===== CATEGORY WITH PRODUCT COUNT DTO =====
    public class CategoryWithProductCountVM
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }

    // ===== CATEGORY WITH PRODUCTS DTO =====
    public class CategoryWithProductsVM
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
        public List<ProductInCategoryVM> Products { get; set; } = new List<ProductInCategoryVM>();
    }

    // ===== PRODUCT IN CATEGORY DTO =====
    public class ProductInCategoryVM
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
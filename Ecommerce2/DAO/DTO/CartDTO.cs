using System.ComponentModel.DataAnnotations;

namespace DAL.DTO
{
    // ===== CREATE DTO =====
    public class CreateCartVM
    {
        public required string UserId { get; set; }
    }

    // ===== ADD TO CART DTO =====
    public class AddToCartVM
    {
        public required string UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be  than 0")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    // ===== UPDATE CART ITEM DTO =====
    public class UpdateCartItemVM
    {
        public int Quantity { get; set; }
    }

    // ===== CART ITEM RESPONSE DTO =====
    public class CartItemVM
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime AddedAt { get; set; }
    }

    // ===== CART RESPONSE DTO =====
    public class CartVM
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public List<CartItemVM> CartItems { get; set; } = new List<CartItemVM>();
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    // ===== CART SUMMARY DTO =====
    public class CartSummaryVM
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
